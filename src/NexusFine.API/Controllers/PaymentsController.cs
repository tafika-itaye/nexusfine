using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;
using NexusFine.Infrastructure.Services;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var year  = DateTime.UtcNow.Year;
        var count = _db.Payments.Count() + 1;
        return $"MPAY-{year}-{count:D5}";
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
