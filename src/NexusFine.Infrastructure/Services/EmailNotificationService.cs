using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// Email notification channel using standard SMTP.
/// For Azure: use Azure Communication Services Email, SendGrid, or Office 365 SMTP.
///
/// Config:
/// "Email": {
///   "Enabled":     true,
///   "SmtpHost":    "smtp.office365.com",
///   "SmtpPort":    587,
///   "UseSsl":      true,
///   "Username":    "notifications@technexusmw.com",
///   "Password":    "…",
///   "FromAddress": "notifications@technexusmw.com",
///   "FromName":    "Malawi Police Service — NexusFine"
/// }
///
/// Without credentials the service logs and no-ops (demo/offline safe).
/// Note: INotificationService only passes phone, not email. Email is resolved
/// via the Driver record in Module 1's service layer before reaching this.
/// For now, phone is treated as a routing id and we swallow email-only calls.
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(
        IConfiguration config,
        ILogger<EmailNotificationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    // All five events delegate to SendAsync with a subject + body built per type.

    public Task SendFineIssuedAsync(string phone, string driverName, string fineRef,
        string offenceName, decimal amount, DateTime dueDate) =>
        SendAsync(phone, driverName,
            subject: $"Traffic fine issued — {fineRef}",
            body: $"Dear {driverName},\n\nA traffic fine has been issued against your vehicle.\n\n" +
                  $"Reference: {fineRef}\nOffence: {offenceName}\nAmount: MK {amount:N0}\n" +
                  $"Due: {dueDate:dd MMM yyyy}\n\nPay at https://nexusfine.mw or dial *NXF#.\n\n" +
                  "— Malawi Police Service (via TechNexus NexusFine)");

    public Task SendPaymentConfirmedAsync(string phone, string driverName, string fineRef,
        string receiptNo, decimal amount) =>
        SendAsync(phone, driverName,
            subject: $"Payment confirmed — Receipt {receiptNo}",
            body: $"Dear {driverName},\n\nYour payment has been confirmed.\n\n" +
                  $"Fine Ref: {fineRef}\nReceipt: {receiptNo}\nAmount: MK {amount:N0}\n\n" +
                  "Keep this email as proof of payment.\n\n— Malawi Police Service");

    public Task SendPaymentReminderAsync(string phone, string driverName, string fineRef,
        decimal amount, int daysUntilDue) =>
        SendAsync(phone, driverName,
            subject: $"Reminder — fine {fineRef} due in {daysUntilDue} day(s)",
            body: $"Dear {driverName},\n\nYour traffic fine {fineRef} is due in {daysUntilDue} day(s).\n" +
                  $"Amount: MK {amount:N0}\n\nPay at https://nexusfine.mw.\n\n— MPS");

    public Task SendOverdueNoticeAsync(string phone, string driverName, string fineRef,
        decimal originalAmount, decimal penaltyAmount) =>
        SendAsync(phone, driverName,
            subject: $"OVERDUE — fine {fineRef}",
            body: $"Dear {driverName},\n\nYour fine {fineRef} is overdue.\n" +
                  $"Original: MK {originalAmount:N0}\nPenalty: MK {penaltyAmount:N0}\n" +
                  $"Total: MK {originalAmount + penaltyAmount:N0}\n\nPay immediately at https://nexusfine.mw.\n\n— MPS");

    public Task SendAppealUpdateAsync(string phone, string driverName, string appealRef,
        string status, string? reviewerNotes)
    {
        var outcome = status == "Upheld"
            ? "Your appeal has been upheld and the fine has been cancelled."
            : $"Outcome: {status}.";
        var note = reviewerNotes is null ? "" : $"\n\nReviewer notes: {reviewerNotes}";
        return SendAsync(phone, driverName,
            subject: $"Appeal {appealRef} — status updated",
            body: $"Dear {driverName},\n\n{outcome}{note}\n\n— MPS");
    }

    // ── CORE SEND ────────────────────────────────────────────
    private Task SendAsync(string phone, string driverName, string subject, string body)
    {
        if (!bool.TryParse(_config["Email:Enabled"], out var enabled) || !enabled)
        {
            _logger.LogDebug("Email disabled — skipping '{Subject}'", subject);
            return Task.CompletedTask;
        }

        // The phone is the only routing key INotificationService gives us.
        // Email resolution (phone → driver.Email) happens in Module 1's service layer.
        // Here we extract an email if the caller packed one into phone ("email:…").
        string? toAddress = null;
        if (phone.StartsWith("email:", StringComparison.OrdinalIgnoreCase))
            toAddress = phone[6..].Trim();

        if (string.IsNullOrWhiteSpace(toAddress))
        {
            _logger.LogDebug("Email skipped — no address for {Driver}", driverName);
            return Task.CompletedTask;
        }

        return DispatchAsync(toAddress, driverName, subject, body);
    }

    private async Task DispatchAsync(string to, string driverName, string subject, string body)
    {
        var host = _config["Email:SmtpHost"];
        var username = _config["Email:Username"];
        var password = _config["Email:Password"];
        var from = _config["Email:FromAddress"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from) ||
            host.StartsWith("REPLACE_") || (username?.StartsWith("REPLACE_") ?? false))
        {
            _logger.LogInformation("Email NO-OP (no credentials) — '{Subject}' for {To}", subject, to);
            return;
        }

        try
        {
            int.TryParse(_config["Email:SmtpPort"], out var port);
            bool.TryParse(_config["Email:UseSsl"], out var ssl);
            using var client = new SmtpClient(host, port > 0 ? port : 587)
            {
                EnableSsl = ssl,
                Credentials = new NetworkCredential(username, password)
            };
            using var msg = new MailMessage(
                new MailAddress(from, _config["Email:FromName"] ?? "NexusFine"),
                new MailAddress(to, driverName))
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            await client.SendMailAsync(msg);
            _logger.LogInformation("Email sent — '{Subject}' to {To}", subject, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email failed — '{Subject}' to {To}", subject, to);
        }
    }
}
