using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// SMS notification service for NexusFine.
///
/// LIVE INTEGRATION NOTES:
/// ─────────────────────────────────────────────────────────────
/// Recommended providers for Malawi:
///   - Africa's Talking (africastalking.com) — best local coverage
///   - Vonage (vonage.com) — global fallback
///   - Direct TNM / Airtel bulk SMS API
///
/// Required config (appsettings.json):
/// "Sms": {
///     "Provider":  "AfricasTalking",
///     "ApiKey":    "YOUR_API_KEY",
///     "Username":  "YOUR_USERNAME",
///     "SenderId":  "NexusFine",
///     "BaseUrl":   "https://api.africastalking.com/version1/messaging"
/// }
/// </summary>
public class SmsService
{
    private readonly HttpClient     _http;
    private readonly IConfiguration _config;
    private readonly ILogger<SmsService> _logger;

    public SmsService(
        IHttpClientFactory factory,
        IConfiguration config,
        ILogger<SmsService> logger)
    {
        _http   = factory.CreateClient("SmsGateway");
        _config = config;
        _logger = logger;
    }

    // ── FINE ISSUED ───────────────────────────────────────────
    public async Task SendFineIssuedAsync(
        string phone,
        string driverName,
        string fineRef,
        string offenceName,
        decimal amount,
        DateTime dueDate)
    {
        var message =
            $"Dear {driverName}, a traffic fine has been issued against your vehicle.\n" +
            $"Ref: {fineRef}\n" +
            $"Offence: {offenceName}\n" +
            $"Amount: MK {amount:N0}\n" +
            $"Due: {dueDate:dd MMM yyyy}\n" +
            $"Pay at: nexusfine.mw or dial *NXF# or WhatsApp +265 1 800 000\n" +
            $"Malawi Police Service";

        await SendAsync(phone, message, "FineIssued");
    }

    // ── PAYMENT CONFIRMED ─────────────────────────────────────
    public async Task SendPaymentConfirmedAsync(
        string phone,
        string driverName,
        string fineRef,
        string receiptNo,
        decimal amount)
    {
        var message =
            $"Payment confirmed. Thank you, {driverName}.\n" +
            $"Fine Ref: {fineRef}\n" +
            $"Receipt: {receiptNo}\n" +
            $"Amount Paid: MK {amount:N0}\n" +
            $"Your fine is fully settled. Keep this SMS as proof.\n" +
            $"Malawi Police Service";

        await SendAsync(phone, message, "PaymentConfirmed");
    }

    // ── PAYMENT REMINDER ──────────────────────────────────────
    public async Task SendPaymentReminderAsync(
        string phone,
        string driverName,
        string fineRef,
        decimal amount,
        int daysUntilDue)
    {
        var urgency = daysUntilDue <= 3 ? "URGENT: " : string.Empty;
        var message =
            $"{urgency}Dear {driverName}, your traffic fine is due in {daysUntilDue} day(s).\n" +
            $"Ref: {fineRef}\n" +
            $"Amount: MK {amount:N0}\n" +
            $"Pay now at nexusfine.mw or dial *NXF#\n" +
            $"Failure to pay may result in additional penalties.\n" +
            $"Malawi Police Service";

        await SendAsync(phone, message, "PaymentReminder");
    }

    // ── OVERDUE NOTICE ────────────────────────────────────────
    public async Task SendOverdueNoticeAsync(
        string phone,
        string driverName,
        string fineRef,
        decimal originalAmount,
        decimal penaltyAmount)
    {
        var message =
            $"NOTICE: Dear {driverName}, your fine {fineRef} is OVERDUE.\n" +
            $"Original Amount: MK {originalAmount:N0}\n" +
            $"Late Penalty:    MK {penaltyAmount:N0}\n" +
            $"Total Due:       MK {originalAmount + penaltyAmount:N0}\n" +
            $"Pay immediately at nexusfine.mw to avoid further action.\n" +
            $"Malawi Police Service";

        await SendAsync(phone, message, "OverdueNotice");
    }

    // ── APPEAL STATUS ─────────────────────────────────────────
    public async Task SendAppealUpdateAsync(
        string phone,
        string driverName,
        string appealRef,
        string status,
        string? reviewerNotes)
    {
        var outcome = status == "Upheld"
            ? "Your appeal has been UPHELD. The fine has been cancelled."
            : $"Your appeal has been reviewed. Outcome: {status}.";

        var message =
            $"Dear {driverName}, your fine appeal {appealRef} has been updated.\n" +
            $"{outcome}\n" +
            (reviewerNotes is not null ? $"Notes: {reviewerNotes}\n" : string.Empty) +
            $"Malawi Police Service";

        await SendAsync(phone, message, "AppealUpdate");
    }

    // ── CORE SEND ─────────────────────────────────────────────
    private async Task SendAsync(string phone, string message, string messageType)
    {
        // Normalise phone — ensure +265 prefix
        var normalisedPhone = NormalisePhone(phone);

        _logger.LogInformation(
            "SMS [{Type}]: Sending to {Phone} — {Chars} chars",
            messageType, normalisedPhone, message.Length);

        try
        {
            // STUB: In production with Africa's Talking:
            //
            // var payload = new FormUrlEncodedContent(new Dictionary<string, string>
            // {
            //     ["username"] = _config["Sms:Username"]!,
            //     ["to"]       = normalisedPhone,
            //     ["message"]  = message,
            //     ["from"]     = _config["Sms:SenderId"] ?? "NexusFine"
            // });
            // _http.DefaultRequestHeaders.Add("apiKey", _config["Sms:ApiKey"]);
            // _http.DefaultRequestHeaders.Add("Accept", "application/json");
            // var response = await _http.PostAsync("", payload);
            // response.EnsureSuccessStatusCode();

            // Simulated success
            await Task.Delay(200);

            _logger.LogInformation(
                "SMS [{Type}]: Sent successfully to {Phone}", messageType, normalisedPhone);
        }
        catch (Exception ex)
        {
            // SMS failure should never block the main payment flow — log and continue
            _logger.LogError(ex,
                "SMS [{Type}]: Failed to send to {Phone}", messageType, normalisedPhone);
        }
    }

    private static string NormalisePhone(string phone)
    {
        phone = phone.Replace(" ", "").Replace("-", "").Trim();

        if (phone.StartsWith("0"))
            phone = "+265" + phone[1..];

        if (!phone.StartsWith("+"))
            phone = "+265" + phone;

        return phone;
    }
}
