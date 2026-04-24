using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// WhatsApp Business Cloud API notification service for NexusFine.
/// Replaces SmsService (removed 2026-04-24).
///
/// LIVE CONFIG (appsettings.json):
/// "WhatsApp": {
///   "BaseUrl":     "https://graph.facebook.com/v20.0/",
///   "PhoneNumberId": "YOUR_WA_PHONE_NUMBER_ID",
///   "AccessToken":   "YOUR_WA_ACCESS_TOKEN",
///   "SenderName":    "NexusFine"
/// }
///
/// For the Minister demo, leave AccessToken empty — the service logs and
/// no-ops (deterministic, no external traffic).
/// </summary>
public class WhatsAppNotificationService : INotificationService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<WhatsAppNotificationService> _logger;

    public WhatsAppNotificationService(
        IHttpClientFactory factory,
        IConfiguration config,
        ILogger<WhatsAppNotificationService> logger)
    {
        _http = factory.CreateClient("WhatsApp");
        _config = config;
        _logger = logger;
    }

    public Task SendFineIssuedAsync(string phone, string driverName, string fineRef,
        string offenceName, decimal amount, DateTime dueDate) =>
        SendAsync(phone,
            $"Dear {driverName}, a traffic fine has been issued against your vehicle. " +
            $"Ref: {fineRef}. Offence: {offenceName}. Amount: MK {amount:N0}. " +
            $"Due: {dueDate:dd MMM yyyy}. Pay at nexusfine.mw, dial *NXF#, or reply here to proceed. " +
            "— Malawi Police Service",
            "FineIssued");

    public Task SendPaymentConfirmedAsync(string phone, string driverName, string fineRef,
        string receiptNo, decimal amount) =>
        SendAsync(phone,
            $"Payment confirmed. Thank you, {driverName}. Fine Ref: {fineRef}. " +
            $"Receipt: {receiptNo}. Amount Paid: MK {amount:N0}. " +
            "Your fine is fully settled. Keep this message as proof. — Malawi Police Service",
            "PaymentConfirmed");

    public Task SendPaymentReminderAsync(string phone, string driverName, string fineRef,
        decimal amount, int daysUntilDue)
    {
        var urgency = daysUntilDue <= 3 ? "URGENT: " : "";
        return SendAsync(phone,
            $"{urgency}Dear {driverName}, your traffic fine {fineRef} is due in {daysUntilDue} day(s). " +
            $"Amount: MK {amount:N0}. Pay now at nexusfine.mw or dial *NXF#. — Malawi Police Service",
            "PaymentReminder");
    }

    public Task SendOverdueNoticeAsync(string phone, string driverName, string fineRef,
        decimal originalAmount, decimal penaltyAmount) =>
        SendAsync(phone,
            $"NOTICE: Dear {driverName}, your fine {fineRef} is OVERDUE. " +
            $"Original: MK {originalAmount:N0}. Late penalty: MK {penaltyAmount:N0}. " +
            $"Total due: MK {originalAmount + penaltyAmount:N0}. Pay at nexusfine.mw to avoid further action. " +
            "— Malawi Police Service",
            "OverdueNotice");

    public Task SendAppealUpdateAsync(string phone, string driverName, string appealRef,
        string status, string? reviewerNotes)
    {
        var outcome = status == "Upheld"
            ? "Your appeal has been UPHELD. The fine has been cancelled."
            : $"Your appeal has been reviewed. Outcome: {status}.";

        var note = reviewerNotes is not null ? $" Notes: {reviewerNotes}." : "";
        return SendAsync(phone,
            $"Dear {driverName}, your fine appeal {appealRef} has been updated. {outcome}{note} " +
            "— Malawi Police Service",
            "AppealUpdate");
    }

    private async Task SendAsync(string phone, string message, string messageType)
    {
        var normalised = NormalisePhone(phone);
        var token = _config["WhatsApp:AccessToken"];
        var phoneId = _config["WhatsApp:PhoneNumberId"];

        _logger.LogInformation("WhatsApp [{Type}] to {Phone} ({Chars} chars)",
            messageType, normalised, message.Length);

        // Demo / offline mode — skip outbound call if no credentials
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(phoneId) ||
            token.StartsWith("REPLACE_"))
        {
            _logger.LogInformation(
                "WhatsApp [{Type}] NO-OP (no credentials — demo/offline mode). Phone={Phone}",
                messageType, normalised);
            await Task.CompletedTask;
            return;
        }

        try
        {
            var payload = new
            {
                messaging_product = "whatsapp",
                to = normalised.TrimStart('+'),
                type = "text",
                text = new { body = message }
            };
            var req = new HttpRequestMessage(HttpMethod.Post, $"{phoneId}/messages");
            req.Headers.Add("Authorization", $"Bearer {token}");
            req.Content = System.Net.Http.Json.JsonContent.Create(payload);
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            _logger.LogInformation("WhatsApp [{Type}] sent to {Phone}", messageType, normalised);
        }
        catch (Exception ex)
        {
            // Notification failure must never break the payment flow
            _logger.LogError(ex, "WhatsApp [{Type}] failed for {Phone}", messageType, normalised);
        }
    }

    private static string NormalisePhone(string phone)
    {
        phone = phone.Replace(" ", "").Replace("-", "").Trim();
        if (phone.StartsWith("0")) phone = "+265" + phone[1..];
        if (!phone.StartsWith("+")) phone = "+265" + phone;
        return phone;
    }
}
