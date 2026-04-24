namespace NexusFine.Infrastructure.Services;

/// <summary>
/// Abstraction for citizen-facing notifications.
/// SMS was removed from scope on 2026-04-24 per client direction.
/// WhatsApp Business API is the primary channel; email is a fallback.
/// A NoOp implementation is supplied for local/offline demo runs.
/// </summary>
public interface INotificationService
{
    Task SendFineIssuedAsync(
        string phone,
        string driverName,
        string fineRef,
        string offenceName,
        decimal amount,
        DateTime dueDate);

    Task SendPaymentConfirmedAsync(
        string phone,
        string driverName,
        string fineRef,
        string receiptNo,
        decimal amount);

    Task SendPaymentReminderAsync(
        string phone,
        string driverName,
        string fineRef,
        decimal amount,
        int daysUntilDue);

    Task SendOverdueNoticeAsync(
        string phone,
        string driverName,
        string fineRef,
        decimal originalAmount,
        decimal penaltyAmount);

    Task SendAppealUpdateAsync(
        string phone,
        string driverName,
        string appealRef,
        string status,
        string? reviewerNotes);
}
