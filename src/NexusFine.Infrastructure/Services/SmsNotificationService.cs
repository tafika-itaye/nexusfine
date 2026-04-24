using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// SMS notification channel (Africa's Talking / Vonage / direct TNM/Airtel bulk).
///
/// Kept as a SHIPPABLE FEATURE even though the DRTSS quotation does not bill
/// for an SMS bundle. Other ministries (or later phases) can enable it by
/// supplying real credentials; without credentials it logs and no-ops so the
/// rest of the system is unaffected.
///
/// Config:
/// "Sms": {
///   "Enabled":  true,
///   "Provider": "AfricasTalking",
///   "ApiKey":   "…",  "Username": "…",  "SenderId": "NexusFine",
///   "BaseUrl":  "https://api.africastalking.com/version1/messaging/"
/// }
/// </summary>
public class SmsNotificationService : INotificationService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<SmsNotificationService> _logger;

    public SmsNotificationService(
        IHttpClientFactory factory,
        IConfiguration config,
        ILogger<SmsNotificationService> logger)
    {
        _http = factory.CreateClient("SmsGateway");
        _config = config;
        _logger = logger;
    }

    public Task SendFineIssuedAsync(string phone, string driverName, string fineRef,
        string offenceName, decimal amount, DateTime dueDate) =>
        SendAsync(phone,
            $"Dear {driverName}, traffic fine issued. Ref: {fineRef}. Offence: {offenceName}. " +
            $"Amount: MK {amount:N0}. Due: {dueDate:dd MMM yyyy}. " +
            "Pay at nexusfine.mw or dial *NXF#. — Malawi Police Service",
            "FineIssued");

    public Task SendPaymentConfirmedAsync(string phone, string driverName, string fineRef,
        string receiptNo, decimal amount) =>
        SendAsync(phone,
            $"Payment confirmed. Thank you, {driverName}. Fine {fineRef}. " +
            $"Receipt {receiptNo}. MK {amount:N0} paid in full. — Malawi Police Service",
            "PaymentConfirmed");

    public Task SendPaymentReminderAsync(string phone, string driverName, string fineRef,
        decimal amount, int daysUntilDue)
    {
        var u = daysUntilDue <= 3 ? "URGENT: " : "";
        return SendAsync(phone,
            $"{u}{driverName}, fine {fineRef} is due in {daysUntilDue} day(s). " +
            $"Amount: MK {amount:N0}. Pay at nexusfine.mw or *NXF#. — MPS",
            "PaymentReminder");
    }

    public Task SendOverdueNoticeAsync(string phone, string driverName, string fineRef,
        decimal originalAmount, decimal penaltyAmount) =>
        SendAsync(phone,
            $"OVERDUE: {driverName}, fine {fineRef}. Original MK {originalAmount:N0}, " +
            $"penalty MK {penaltyAmount:N0}, total MK {originalAmount + penaltyAmount:N0}. " +
            "Pay at nexusfine.mw. — MPS",
            "OverdueNotice");

    public Task SendAppealUpdateAsync(string phone, string driverName, string appealRef,
        string status, string? reviewerNotes)
    {
        var outcome = status == "Upheld" ? "UPHELD — fine cancelled." : $"Outcome: {status}.";
        var note = reviewerNotes is null ? "" : $" Notes: {reviewerNotes}.";
        return SendAsync(phone,
            $"{driverName}, appeal {appealRef} updated. {outcome}{note} — MPS",
            "AppealUpdate");
    }

    private async Task SendAsync(string phone, string message, string messageType)
    {
        if (!bool.TryParse(_config["Sms:Enabled"], out var enabled) || !enabled)
        {
            _logger.LogDebug("SMS disabled — skipping {Type}", messageType);
            return;
        }

        var normalised = NormalisePhone(phone);
        var apiKey = _config["Sms:ApiKey"];
        var username = _config["Sms:Username"];

        _logger.LogInformation("SMS [{Type}] to {Phone} ({Chars} chars)",
            messageType, normalised, message.Length);

        // Demo / offline mode
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.StartsWith("REPLACE_") ||
            string.IsNullOrWhiteSpace(username) || username.StartsWith("REPLACE_"))
        {
            _logger.LogInformation("SMS [{Type}] NO-OP (no credentials)", messageType);
            return;
        }

        try
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = username,
                ["to"] = normalised,
                ["message"] = message,
                ["from"] = _config["Sms:SenderId"] ?? "NexusFine"
            });
            var req = new HttpRequestMessage(HttpMethod.Post, "")
            { Content = content };
            req.Headers.Add("apiKey", apiKey);
            req.Headers.Add("Accept", "application/json");
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            _logger.LogInformation("SMS [{Type}] sent to {Phone}", messageType, normalised);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMS [{Type}] failed for {Phone}", messageType, normalised);
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
