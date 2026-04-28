using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

/// <summary>
/// Demo-quality chat-bot backend used by the WhatsApp + USSD simulators in
/// the admin portal. Implements the same state machine for both channels;
/// the visual presentation differs only in the front-end. Session state is
/// held in-memory (ConcurrentDictionary) — this is a demo surface, not a
/// production WhatsApp Business API integration. Real wiring to Meta /
/// Africa's Talking USSD lands post-pilot.
/// </summary>
[ApiController]
[Route("api/chatbot")]
[AllowAnonymous]                                    // demo surface; the citizen channel is unauth'd by design
public class ChatBotController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, ChatSession> _sessions = new();
    private readonly AppDbContext _db;

    public ChatBotController(AppDbContext db) => _db = db;

    // ── ENTRY ──
    // POST api/chatbot/{channel}/start
    [HttpPost("{channel}/start")]
    public IActionResult Start(string channel)
    {
        var sessionId = Guid.NewGuid().ToString("N")[..10];
        var session   = new ChatSession { Id = sessionId, Channel = channel.ToLower(), State = State.Welcome };
        _sessions[sessionId] = session;

        return Ok(new ChatTurn(
            sessionId,
            session.State.ToString(),
            Welcome(channel),
            QuickReplies("1", "2", "3", "4")));
    }

    // POST api/chatbot/{channel}/message
    [HttpPost("{channel}/message")]
    public async Task<IActionResult> Message(string channel, [FromBody] InboundMessage req)
    {
        if (string.IsNullOrEmpty(req.SessionId) || !_sessions.TryGetValue(req.SessionId, out var s))
            return BadRequest(new { message = "Unknown session — call /start first." });

        var input = (req.Text ?? "").Trim();
        var reply = await Advance(s, input);

        return Ok(new ChatTurn(s.Id, s.State.ToString(), reply.Bubbles, reply.QuickReplies));
    }

    // ── STATE MACHINE ──
    private async Task<(List<Bubble> Bubbles, List<string> QuickReplies)> Advance(ChatSession s, string input)
    {
        var lc = input.ToLowerInvariant();
        if (lc is "menu" or "0") { s.State = State.Welcome; return (Welcome(s.Channel), QuickReplies("1","2","3","4")); }

        switch (s.State)
        {
            case State.Welcome:
                return input switch
                {
                    "1" => Move(s, State.AwaitingPlate,
                        new[] { "🔎 Please send your *vehicle plate number* (e.g. MW-LL-1234)." }),
                    "2" => Move(s, State.AwaitingReference,
                        new[] { "📑 Please send your *fine reference number* (e.g. NXF-2026-000142)." }),
                    "3" => Move(s, State.AwaitingLicence,
                        new[] { "🪪 Please send your *driver's licence number*." }),
                    "4" => Move(s, State.Welcome,
                        new[] {
                            "*NexusFine help*",
                            "• Lookup any fine using plate, reference or licence",
                            "• Pay through Airtel Money, TNM Mpamba, or USSD *244#",
                            "• Receipts arrive here as PDF",
                            "Reply *menu* to start over."
                        },
                        QuickReplies("1","2","3","4")),
                    _ => (Reprompt("Please reply with 1, 2, 3 or 4."), QuickReplies("1","2","3","4"))
                };

            case State.AwaitingPlate:
                return await LookupAndShow(s, plate: input);

            case State.AwaitingReference:
                return await LookupAndShow(s, reference: input);

            case State.AwaitingLicence:
                return await LookupAndShow(s, licence: input);

            case State.ShowingFines:
                if (int.TryParse(input, out var idx) && idx >= 1 && idx <= s.LastFineIds.Count)
                {
                    s.SelectedFineId = s.LastFineIds[idx - 1];
                    s.State = State.AwaitingChannel;
                    return (
                        new List<Bubble>
                        {
                            Bubble.Bot($"How would you like to pay?\n\n1. Airtel Money\n2. TNM Mpamba\n3. Bank transfer\n4. Card\n5. USSD *244#"),
                        },
                        QuickReplies("1","2","3","4","5"));
                }
                return (Reprompt("Reply with the number of the fine you'd like to pay (or *menu* to start over)."), new());

            case State.AwaitingChannel:
                var channelMap = new Dictionary<string, PaymentChannel>
                {
                    ["1"] = PaymentChannel.AirtelMoney,
                    ["2"] = PaymentChannel.TnmMpamba,
                    ["3"] = PaymentChannel.BankTransfer,
                    ["4"] = PaymentChannel.Card,
                    ["5"] = PaymentChannel.Ussd
                };
                if (channelMap.TryGetValue(input, out var ch))
                {
                    return await InitiatePayment(s, ch);
                }
                return (Reprompt("Please pick 1-5."), QuickReplies("1","2","3","4","5"));

            case State.AwaitingPhoneForPayment:
                if (input.Length >= 9 && input.All(c => char.IsDigit(c) || c == '+'))
                {
                    s.PayerPhone = input;
                    return await CompletePayment(s);
                }
                return (Reprompt("That doesn't look like a valid phone. Try again — e.g. 0991234567."), new());

            case State.Done:
                s.State = State.Welcome;
                return (Welcome(s.Channel), QuickReplies("1","2","3","4"));
        }

        return (Reprompt("Sorry, I didn't follow. Reply *menu* to restart."), new());
    }

    private async Task<(List<Bubble>, List<string>)> LookupAndShow(
        ChatSession s, string? plate = null, string? reference = null, string? licence = null)
    {
        IQueryable<Fine> q = _db.Fines.Include(f => f.OffenceCode);
        if (!string.IsNullOrWhiteSpace(plate))     q = q.Where(f => f.PlateNumber.Contains(plate));
        if (!string.IsNullOrWhiteSpace(reference)) q = q.Where(f => f.ReferenceNumber == reference);
        if (!string.IsNullOrWhiteSpace(licence))   q = q.Where(f => f.DriverLicenceNumber == licence);

        var fines = await q.Where(f => f.Status != FineStatus.Cancelled)
                           .OrderByDescending(f => f.IssuedAt)
                           .Take(5).ToListAsync();

        if (!fines.Any())
        {
            s.State = State.Welcome;
            return (
                new List<Bubble> { Bubble.Bot("✅ No outstanding fines found for that detail.\n\nReply *menu* to do another lookup.") },
                QuickReplies("menu"));
        }

        s.LastFineIds = fines.Select(f => f.Id).ToList();
        s.State = State.ShowingFines;

        var lines = new List<string> { $"Found *{fines.Count}* fine(s):" };
        for (int i = 0; i < fines.Count; i++)
        {
            var f = fines[i];
            lines.Add($"\n*{i + 1}.* `{f.ReferenceNumber}`\n• {f.OffenceCode.Name} — MK {f.Amount:N0}\n• Plate {f.PlateNumber} · {f.Status}");
        }
        lines.Add("\nReply with the *number* of the fine to pay it.");

        return (
            new List<Bubble> { Bubble.Bot(string.Join("\n", lines)) },
            QuickReplies(Enumerable.Range(1, fines.Count).Select(i => i.ToString()).ToArray()));
    }

    private async Task<(List<Bubble>, List<string>)> InitiatePayment(ChatSession s, PaymentChannel ch)
    {
        s.SelectedChannel = ch;

        // Cards / Bank transfer follow a different flow; for the demo route them through
        // a shortened "use the citizen portal" path. Mobile money + USSD ask for a phone here.
        if (ch is PaymentChannel.AirtelMoney or PaymentChannel.TnmMpamba or PaymentChannel.Ussd)
        {
            s.State = State.AwaitingPhoneForPayment;
            return (
                new List<Bubble> { Bubble.Bot("📱 Please send the *phone number* to charge for this payment.") },
                new());
        }

        return await CompletePayment(s);
    }

    private async Task<(List<Bubble>, List<string>)> CompletePayment(ChatSession s)
    {
        var fine = await _db.Fines.FirstOrDefaultAsync(f => f.Id == s.SelectedFineId);
        if (fine is null)
        {
            s.State = State.Done;
            return (new List<Bubble> { Bubble.Bot("⚠ Sorry, we couldn't find that fine any more. Reply *menu* to start over.") }, new());
        }

        // For the demo we skip the actual PaymentsController.Initiate call to keep this
        // controller self-contained; in production the chatbot would forward to it.
        var receipt = $"MPAY-CHAT-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        var msg = $@"✅ *Payment received*

Receipt: `{receipt}`
Fine:    `{fine.ReferenceNumber}`
Amount:  MK {fine.Amount:N0}
Channel: {s.SelectedChannel}
Status:  COMPLETED

A PDF receipt has been sent to your inbox.

Reply *menu* to do another lookup.";

        s.State = State.Done;
        return (new List<Bubble> { Bubble.Bot(msg) }, QuickReplies("menu"));
    }

    // ── HELPERS ──
    private static (List<Bubble>, List<string>) Move(ChatSession s, State next, string[] lines, List<string>? quick = null)
    {
        s.State = next;
        return (lines.Select(Bubble.Bot).ToList(), quick ?? new());
    }

    private static List<Bubble> Reprompt(string text) => new() { Bubble.Bot(text) };

    private static List<Bubble> Welcome(string channel)
    {
        var hello = channel == "ussd"
            ? "*NexusFine — DRTSS*\nMalawi traffic-fines portal\n\n1. Look up by plate\n2. Look up by reference\n3. Look up by licence\n4. Help\n\nReply 0 for menu."
            : "👋 *Welcome to NexusFine*\nMalawi Police Service · DRTSS\n\nHow can I help?\n\n*1.* Look up a fine by plate\n*2.* Look up by reference number\n*3.* Look up by driver's licence\n*4.* Help & FAQs\n\nReply *menu* anytime to come back here.";
        return new List<Bubble> { Bubble.Bot(hello) };
    }

    private static List<string> QuickReplies(params string[] options) => options.ToList();

    // ── DTOs / state ──
    public record InboundMessage(string SessionId, string Text);
    public record ChatTurn(string SessionId, string State, List<Bubble> Replies, List<string> QuickReplies);
    public record Bubble(string From, string Text)
    {
        public static Bubble Bot(string text)  => new("bot", text);
        public static Bubble User(string text) => new("user", text);
    }

    private enum State
    {
        Welcome,
        AwaitingPlate,
        AwaitingReference,
        AwaitingLicence,
        ShowingFines,
        AwaitingChannel,
        AwaitingPhoneForPayment,
        Done
    }

    private class ChatSession
    {
        public string Id { get; set; } = "";
        public string Channel { get; set; } = "whatsapp";
        public State State { get; set; }
        public List<int> LastFineIds { get; set; } = new();
        public int? SelectedFineId { get; set; }
        public PaymentChannel? SelectedChannel { get; set; }
        public string? PayerPhone { get; set; }
    }
}
