namespace NexusFine.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty; // MPAY-2026-88123

    public int FineId { get; set; }
    public Fine Fine { get; set; } = null!;

    public decimal Amount { get; set; }
    public PaymentChannel Channel { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Mobile money / bank refs
    public string? PhoneNumber { get; set; }
    public string? TransactionReference { get; set; } // gateway ref
    public string? GatewayResponse { get; set; }      // raw JSON from gateway

    public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum PaymentChannel
{
    AirtelMoney = 0,
    TnmMpamba = 1,
    BankTransfer = 2,
    Card = 3,
    Ussd = 4,
    WhatsApp = 5,
    Cash = 6      // counter payment fallback
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Reversed = 3,
    TimedOut = 4
}
