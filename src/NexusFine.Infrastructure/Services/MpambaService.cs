using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// TNM Mpamba payment gateway integration.
///
/// LIVE INTEGRATION NOTES (for when MPS provides credentials):
/// ─────────────────────────────────────────────────────────────
/// TNM Mpamba uses the Flutterwave API or a direct TNM Business API.
/// Contact: TNM Business Solutions · +265 1 321 000
/// Docs:    https://developer.tnmmobile.com (request access from TNM)
///
/// Required config (appsettings.json):
/// "TnmMpamba": {
///     "ApiKey":       "YOUR_API_KEY",
///     "ApiSecret":    "YOUR_SECRET",
///     "MerchantCode": "YOUR_MERCHANT_CODE",
///     "BaseUrl":      "https://api.tnmmobile.com/v1",
///     "CallbackUrl":  "https://your-domain/api/payments/callback/mpamba"
/// }
/// </summary>
public class MpambaService : IPaymentGateway
{
    private readonly HttpClient     _http;
    private readonly IConfiguration _config;
    private readonly ILogger<MpambaService> _logger;

    public MpambaService(
        IHttpClientFactory factory,
        IConfiguration config,
        ILogger<MpambaService> logger)
    {
        _http   = factory.CreateClient("TnmMpamba");
        _config = config;
        _logger = logger;
    }

    // ── INITIATE ──────────────────────────────────────────────
    public async Task<GatewayResult> InitiateAsync(GatewayRequest request)
    {
        _logger.LogInformation(
            "Mpamba: Initiating payment for {Phone}, amount MWK {Amount}, ref {Ref}",
            request.Phone, request.Amount, request.Reference);

        try
        {
            // STUB: In production, POST to TNM Mpamba endpoint:
            //
            // var payload = new {
            //     msisdn          = request.Phone,
            //     amount          = request.Amount,
            //     external_ref    = request.Reference,
            //     description     = request.Description,
            //     callback_url    = request.CallbackUrl,
            //     merchant_code   = _config["TnmMpamba:MerchantCode"]
            // };
            // _http.DefaultRequestHeaders.Add("X-Api-Key", _config["TnmMpamba:ApiKey"]);
            // var response = await _http.PostAsJsonAsync("payments/collect", payload);

            await Task.Delay(500);

            var txRef = $"MP-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";

            _logger.LogInformation("Mpamba: Payment initiated. TxRef: {TxRef}", txRef);

            return new GatewayResult
            {
                Success              = true,
                TransactionReference = txRef,
                GatewayReference     = $"MPAMBA-{Guid.NewGuid():N}"[..18].ToUpper(),
                Status               = "Pending",
                Message              = $"Mpamba prompt sent to {request.Phone}. Enter PIN to confirm.",
                RawResponse          = JsonSerializer.Serialize(new
                {
                    success     = true,
                    transaction = new { ref_id = txRef, status = "pending" },
                    message     = "Payment initiated"
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mpamba: Payment initiation failed for {Ref}", request.Reference);
            return new GatewayResult
            {
                Success = false,
                Message = $"TNM Mpamba gateway error: {ex.Message}"
            };
        }
    }

    // ── QUERY STATUS ──────────────────────────────────────────
    public async Task<GatewayResult> QueryStatusAsync(string transactionReference)
    {
        _logger.LogInformation("Mpamba: Querying status for {TxRef}", transactionReference);

        try
        {
            // STUB: In production:
            // var response = await _http.GetAsync($"payments/{transactionReference}/status");

            await Task.Delay(400);

            return new GatewayResult
            {
                Success              = true,
                TransactionReference = transactionReference,
                Status               = "Success",
                Message              = "Payment confirmed by Mpamba.",
                RawResponse          = JsonSerializer.Serialize(new
                {
                    success     = true,
                    transaction = new { ref_id = transactionReference, status = "completed" }
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mpamba: Status query failed for {TxRef}", transactionReference);
            return new GatewayResult { Success = false, Message = ex.Message };
        }
    }

    // ── CALLBACK ──────────────────────────────────────────────
    public async Task<GatewayCallbackResult> HandleCallbackAsync(string rawPayload)
    {
        _logger.LogInformation("Mpamba: Received callback payload");

        await Task.CompletedTask;

        try
        {
            // STUB: In production, parse TNM's callback:
            // {
            //   "success": true,
            //   "transaction": {
            //     "ref_id":  "MP-20260408-123456",
            //     "status":  "completed",
            //     "amount":  50000,
            //     "msisdn":  "265880000000"
            //   }
            // }

            var doc = JsonDocument.Parse(rawPayload);
            var success = doc.RootElement.GetProperty("success").GetBoolean();
            var tx      = doc.RootElement.GetProperty("transaction");
            var txId    = tx.GetProperty("ref_id").GetString();
            var status  = tx.GetProperty("status").GetString();
            var amount  = tx.TryGetProperty("amount", out var amt) ? amt.GetDecimal() : 0m;

            return new GatewayCallbackResult
            {
                IsPaymentConfirmed   = success && status == "completed",
                TransactionReference = txId,
                Amount               = amount,
                RawPayload           = rawPayload
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mpamba: Failed to parse callback payload");
            return new GatewayCallbackResult { IsPaymentConfirmed = false, RawPayload = rawPayload };
        }
    }
}
