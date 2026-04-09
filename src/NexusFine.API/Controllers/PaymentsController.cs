using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PaymentsController(AppDbContext db) => _db = db;

    // POST api/payments/initiate
    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest req)
    {
        var fine = await _db.Fines
            .Include(f => f.Payments)
            .FirstOrDefaultAsync(f => f.ReferenceNumber == req.FineRef);

        if (fine is null)
            return NotFound(new { message = "Fine not found." });

        if (fine.Status == FineStatus.Paid)
            return BadRequest(new { message = "This fine has already been paid." });

        if (fine.Status == FineStatus.Cancelled)
            return BadRequest(new { message = "This fine has been cancelled." });

        // Create pending payment record
        var payment = new Payment
        {
            ReceiptNumber        = GenerateReceipt(),
            FineId               = fine.Id,
            Amount               = fine.Amount + (fine.PenaltyAmount ?? 0),
            Channel              = req.Channel,
            Status               = PaymentStatus.Pending,
            PhoneNumber          = req.Phone,
            TransactionReference = Guid.NewGuid().ToString("N")[..16].ToUpper(),
            InitiatedAt          = DateTime.UtcNow
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        // TODO: call Airtel / Mpamba / card gateway here
        // For now simulate success after record creation

        return Ok(new
        {
            payment.ReceiptNumber,
            payment.TransactionReference,
            payment.Amount,
            payment.Channel,
            payment.Status,
            message = $"Payment prompt sent to {req.Phone}. Awaiting confirmation."
        });
    }

    // POST api/payments/confirm  (called by gateway callback)
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmPaymentRequest req)
    {
        var payment = await _db.Payments
            .Include(p => p.Fine)
            .FirstOrDefaultAsync(p => p.TransactionReference == req.TransactionReference);

        if (payment is null)
            return NotFound(new { message = "Payment transaction not found." });

        if (payment.Status == PaymentStatus.Completed)
            return Ok(new { message = "Already confirmed.", payment.ReceiptNumber });

        payment.Status      = PaymentStatus.Completed;
        payment.CompletedAt = DateTime.UtcNow;
        payment.UpdatedAt   = DateTime.UtcNow;
        payment.GatewayResponse = req.GatewayPayload;

        // Mark fine as paid
        payment.Fine.Status    = FineStatus.Paid;
        payment.Fine.UpdatedAt = DateTime.UtcNow;

        // Audit log
        _db.AuditLogs.Add(new AuditLog
        {
            EntityType = "Payment",
            EntityId   = payment.Id,
            Action     = "Completed",
            NewValues  = $"{{\"receipt\":\"{payment.ReceiptNumber}\",\"amount\":{payment.Amount}}}",
            Timestamp  = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        // TODO: send SMS receipt to driver

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
