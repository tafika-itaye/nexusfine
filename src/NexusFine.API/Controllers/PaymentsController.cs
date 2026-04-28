using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;
using NexusFine.Infrastructure.Services;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // citizen-facing payment journey; gateway callbacks must reach us unauthenticated
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext          _db;
    private readonly PaymentGatewayFactory _gatewayFactory;
    private readonly INotificationService  _notify;

    public PaymentsController(
        AppDbContext db,
        PaymentGatewayFactory gatewayFactory,
        INotificationService notify)
    {
        _db             = db;
        _gatewayFactory = gatewayFactory;
        _notify         = notify;
    }

    // POST api/payments/initiate
    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest req)
    {
        var fine = await _db.Fines
            .Include(f => f.OffenceCode)
            .Include(f => f.Payments)
            .FirstOrDefaultAsync(f => f.ReferenceNumber == req.FineRef);

        if (fine is null)
            return NotFound(new { message = "Fine not found." });

        if (fine.Status == FineStatus.Paid)
            return BadRequest(new { message = "This fine has already been paid." });

        if (fine.Status == FineStatus.Cancelled)
            return BadRequest(new { message = "This fine has been cancelled." });

        // Only mobile money channels go through gateway
        // Bank and card are handled externally
        GatewayResult? gatewayResult = null;
        if (req.Channel == PaymentChannel.AirtelMoney || req.Channel == PaymentChannel.TnmMpamba)
        {
            var gateway = _gatewayFactory.GetGateway(req.Channel);
            gatewayResult = await gateway.InitiateAsync(new GatewayRequest
            {
                Phone       = req.Phone ?? string.Empty,
                Amount      = fine.Amount + (fine.PenaltyAmount ?? 0),
                Reference   = fine.ReferenceNumber,
                Description = $"Traffic fine {fine.ReferenceNumber} — {fine.OffenceCode?.Name}",
                CallbackUrl = $"{Request.Scheme}://{Request.Host}/api/payments/callback/{req.Channel.ToString().ToLower()}"
            });

            if (gatewayResult is { Success: false })
                return BadRequest(new { message = gatewayResult.Message });
        }

        // Create pending payment record
        var payment = new Payment
        {
            ReceiptNumber        = GenerateReceipt(),
            FineId               = fine.Id,
            Amount               = fine.Amount + (fine.PenaltyAmount ?? 0),
            Channel              = req.Channel,
            Status               = PaymentStatus.Pending,
            PhoneNumber          = req.Phone,
            TransactionReference = gatewayResult?.TransactionReference
                                   ?? Guid.NewGuid().ToString("N")[..16].ToUpper(),
            GatewayResponse      = gatewayResult?.RawResponse,
            InitiatedAt          = DateTime.UtcNow
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            payment.ReceiptNumber,
            payment.TransactionReference,
            payment.Amount,
            payment.Channel,
            payment.Status,
            message = gatewayResult?.Message
                      ?? $"Payment record created. Reference: {payment.TransactionReference}"
        });
    }

    // POST api/payments/callback/airtel
    [HttpPost("callback/airtel")]
    public async Task<IActionResult> AirtelCallback()
    {
        var raw = await new StreamReader(Request.Body).ReadToEndAsync();
        return await ProcessCallback(raw, PaymentChannel.AirtelMoney);
    }

    // POST api/payments/callback/mpamba
    [HttpPost("callback/mpamba")]
    public async Task<IActionResult> MpambaCallback()
    {
        var raw = await new StreamReader(Request.Body).ReadToEndAsync();
        return await ProcessCallback(raw, PaymentChannel.TnmMpamba);
    }

    // POST api/payments/confirm (manual confirm for bank/card or testing)
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmPaymentRequest req)
    {
        var payment = await _db.Payments
            .Include(p => p.Fine)
                .ThenInclude(f => f.OffenceCode)
            .FirstOrDefaultAsync(p => p.TransactionReference == req.TransactionReference);

        if (payment is null)
            return NotFound(new { message = "Payment transaction not found." });

        if (payment.Status == PaymentStatus.Completed)
            return Ok(new { message = "Already confirmed.", payment.ReceiptNumber });

        await ConfirmPaymentAsync(payment, req.GatewayPayload);

        return Ok(new
        {
            payment.ReceiptNumber,
            payment.Amount,
            payment.Channel,
            payment.CompletedAt,
            FineReference = payment.Fine.ReferenceNumber,
            FineStatus    = payment.Fine.Status
        });
    }

    // GET api/payments/{receiptNumber}
    [HttpGet("{receiptNumber}")]
    public async Task<IActionResult> GetByReceipt(string receiptNumber)
    {
        var payment = await _db.Payments
            .Include(p => p.Fine)
            .FirstOrDefaultAsync(p => p.ReceiptNumber == receiptNumber);

        if (payment is null) return NotFound();

        return Ok(new
        {
            payment.ReceiptNumber,
            payment.Amount,
            payment.Channel,
            payment.Status,
            payment.PhoneNumber,
            payment.InitiatedAt,
            payment.CompletedAt,
            Fine = new
            {
                payment.Fine.ReferenceNumber,
                payment.Fine.PlateNumber,
                payment.Fine.DriverName,
                payment.Fine.Status
            }
        });
    }

    // GET api/payments?channel=&status=&from=&to=&q=&page=&pageSize=
    [HttpGet]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string?   channel,
        [FromQuery] string?   status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string?   q,
        [FromQuery] int       page     = 1,
        [FromQuery] int       pageSize = 25)
    {
        if (page     < 1)   page     = 1;
        if (pageSize < 1)   pageSize = 25;
        if (pageSize > 200) pageSize = 200;

        IQueryable<Payment> query = _db.Payments.Include(p => p.Fine);

        if (!string.IsNullOrWhiteSpace(channel) &&
            Enum.TryParse<PaymentChannel>(channel, true, out var ch))
            query = query.Where(p => p.Channel == ch);

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<PaymentStatus>(status, true, out var st))
            query = query.Where(p => p.Status == st);

        if (from.HasValue) query = query.Where(p => p.InitiatedAt >= from.Value);
        if (to.HasValue)   query = query.Where(p => p.InitiatedAt <  to.Value);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim();
            query = query.Where(p =>
                p.ReceiptNumber.Contains(s) ||
                (p.PhoneNumber != null && p.PhoneNumber.Contains(s)) ||
                p.Fine.ReferenceNumber.Contains(s) ||
                p.Fine.PlateNumber.Contains(s));
        }

        var total = await query.CountAsync();

        var rows = await query
            .OrderByDescending(p => p.InitiatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.ReceiptNumber,
                FineReference = p.Fine.ReferenceNumber,
                PlateNumber   = p.Fine.PlateNumber,
                p.Amount,
                p.Channel,
                p.Status,
                p.PhoneNumber,
                p.TransactionReference,
                p.InitiatedAt,
                p.CompletedAt
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items = rows });
    }

    // ── HELPERS ───────────────────────────────────────────────
    private async Task<IActionResult> ProcessCallback(string rawPayload, PaymentChannel channel)
    {
        try
        {
            var gateway  = _gatewayFactory.GetGateway(channel);
            var result   = await gateway.HandleCallbackAsync(rawPayload);

            if (!result.IsPaymentConfirmed || result.TransactionReference is null)
                return Ok(); // acknowledge receipt even if not confirmed

            var payment = await _db.Payments
                .Include(p => p.Fine)
                    .ThenInclude(f => f.OffenceCode)
                .FirstOrDefaultAsync(p => p.TransactionReference == result.TransactionReference);

            if (payment is null || payment.Status == PaymentStatus.Completed)
                return Ok();

            await ConfirmPaymentAsync(payment, rawPayload);
            return Ok();
        }
        catch
        {
            return Ok(); // always return 200 to gateway to prevent retries
        }
    }

    private async Task ConfirmPaymentAsync(Payment payment, string? gatewayPayload)
    {
        payment.Status          = PaymentStatus.Completed;
        payment.CompletedAt     = DateTime.UtcNow;
        payment.UpdatedAt       = DateTime.UtcNow;
        payment.GatewayResponse = gatewayPayload;

        payment.Fine.Status    = FineStatus.Paid;
        payment.Fine.UpdatedAt = DateTime.UtcNow;

        _db.AuditLogs.Add(new AuditLog
        {
            EntityType = "Payment",
            EntityId   = payment.Id,
            Action     = "Completed",
            NewValues  = $"{{\"receipt\":\"{payment.ReceiptNumber}\",\"amount\":{payment.Amount},\"channel\":\"{payment.Channel}\"}}",
            IpAddress  = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Timestamp  = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        // Notify citizen via WhatsApp (SMS removed 2026-04-24)
        if (!string.IsNullOrWhiteSpace(payment.Fine.DriverPhone))
        {
            await _notify.SendPaymentConfirmedAsync(
                phone:      payment.Fine.DriverPhone,
                driverName: payment.Fine.DriverName,
                fineRef:    payment.Fine.ReferenceNumber,
                receiptNo:  payment.ReceiptNumber,
                amount:     payment.Amount
            );
        }
    }

    private string GenerateReceipt()
    {
        // Generate a unique receipt by reading the highest existing ordinal
        // for THIS year's prefix and incrementing. Falls back to a Guid suffix
        // if for any reason a collision still happens (e.g. concurrent creates).
        var year   = DateTime.UtcNow.Year;
        var prefix = $"MPAY-{year}-";

        var existingForYear = _db.Payments
            .Where(p => p.ReceiptNumber.StartsWith(prefix))
            .Select(p => p.ReceiptNumber)
            .ToList();

        var maxOrdinal = existingForYear
            .Select(r =>
            {
                var tail = r.Substring(prefix.Length);
                return int.TryParse(tail, out var n) ? n : 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        var candidate = $"{prefix}{maxOrdinal + 1:D5}";

        // Belt-and-braces: if somehow it collides (race), append a 4-char Guid.
        if (_db.Payments.Any(p => p.ReceiptNumber == candidate))
            candidate = $"{prefix}{maxOrdinal + 1:D5}-{Guid.NewGuid().ToString("N")[..4].ToUpper()}";

        return candidate;
    }
}

// ── REQUEST DTOs ──────────────────────────────────────────────
public record InitiatePaymentRequest(
    string         FineRef,
    PaymentChannel Channel,
    string?        Phone
);

public record ConfirmPaymentRequest(
    string  TransactionReference,
    string? GatewayPayload
);
