using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// Fans out every notification event to every enabled channel in parallel.
/// Failure of one channel never blocks the others or the calling flow.
/// Channel enable/disable is driven by config:
///   WhatsApp:Enabled, Sms:Enabled, Email:Enabled
/// </summary>
public class CompositeNotificationService : INotificationService
{
    private readonly IReadOnlyList<INotificationService> _channels;
    private readonly ILogger<CompositeNotificationService> _logger;

    public CompositeNotificationService(
        IConfiguration config,
        ILogger<CompositeNotificationService> logger,
        WhatsAppNotificationService whatsApp,
        SmsNotificationService sms,
        EmailNotificationService email)
    {
        _logger = logger;
        var list = new List<INotificationService>();

        // WhatsApp is default-enabled for NexusFine
        if (IsEnabled(config, "WhatsApp", defaultEnabled: true)) list.Add(whatsApp);
        if (IsEnabled(config, "Sms",      defaultEnabled: false)) list.Add(sms);
        if (IsEnabled(config, "Email",    defaultEnabled: false)) list.Add(email);

        _channels = list;
        _logger.LogInformation("Notification channels active: {Count}", _channels.Count);
    }

    private static bool IsEnabled(IConfiguration c, string section, bool defaultEnabled)
    {
        var v = c[$"{section}:Enabled"];
        if (string.IsNullOrWhiteSpace(v)) return defaultEnabled;
        return bool.TryParse(v, out var b) ? b : defaultEnabled;
    }

    // Each method fans out to every channel with its own try/catch so a single
    // channel failure never cancels the others.
    public Task SendFineIssuedAsync(string phone, string driverName, string fineRef,
        string offenceName, decimal amount, DateTime dueDate) =>
        FanOut(c => c.SendFineIssuedAsync(phone, driverName, fineRef, offenceName, amount, dueDate));

    public Task SendPaymentConfirmedAsync(string phone, string driverName, string fineRef,
        string receiptNo, decimal amount) =>
        FanOut(c => c.SendPaymentConfirmedAsync(phone, driverName, fineRef, receiptNo, amount));

    public Task SendPaymentReminderAsync(string phone, string driverName, string fineRef,
        decimal amount, int daysUntilDue) =>
        FanOut(c => c.SendPaymentReminderAsync(phone, driverName, fineRef, amount, daysUntilDue));

    public Task SendOverdueNoticeAsync(string phone, string driverName, string fineRef,
        decimal originalAmount, decimal penaltyAmount) =>
        FanOut(c => c.SendOverdueNoticeAsync(phone, driverName, fineRef, originalAmount, penaltyAmount));

    public Task SendAppealUpdateAsync(string phone, string driverName, string appealRef,
        string status, string? reviewerNotes) =>
        FanOut(c => c.SendAppealUpdateAsync(phone, driverName, appealRef, status, reviewerNotes));

    private async Task FanOut(Func<INotificationService, Task> action)
    {
        var tasks = _channels.Select(async c =>
        {
            try { await action(c); }
            catch (Exception ex)
            { _logger.LogError(ex, "Notification channel {Type} threw", c.GetType().Name); }
        });
        await Task.WhenAll(tasks);
    }
}
