using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// Airtel Money Malawi payment gateway integration.
///
/// LIVE INTEGRATION NOTES (for when MPS provides credentials):
/// ─────────────────────────────────────────────────────────────
/// Base URL (sandbox):    https://openapi.airtel.africa/
/// Base URL (production): https://openapi.airtel.africa/
/// Auth endpoint:         POST /auth/oauth2/token
/// Payment endpoint:      POST /merchant/v2/payments/
/// Status endpoint:       GET  /standard/v1/payments/{transactionId}
/// Docs:                  https://developers.airtel.africa/
///
/// Required config (appsettings.json):
/// "AirtelMoney": {
///     "ClientId":     "YOUR_CLIENT_ID",
///     "ClientSecret": "YOUR_CLIENT_SECRET",
///     "PinEncryptionKey": "YOUR_PIN_KEY",
///     "BaseUrl":      "https://openapi.airtel.africa",
///     "CountryCode":  "MW",
///     "Currency":     "MWK",
///     "CallbackUrl":  "https://your-domain/api/payments/callback/airtel"
/// }
/// </summary>
public class AirtelMoneyService : IPaymentGateway
{
    private readonly HttpClient      _http;
    private readonly IConfiguration  _config;
    private readonly ILogger<AirtelMoneyService> _logger;

    private string? _accessToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public AirtelMoneyService(
        IHttpClientFactory factory,
        IConfiguration config,
        ILogger<AirtelMoneyService> logger)
    {
        _http   = factory.CreateClient("AirtelMoney");
        _config = config;
        _logger = logger;
    }

    // ── INITIATE ──────────────────────────────────────────────
    public async Task<GatewayResult> InitiateAsync(GatewayRequest request)
    {
        _logger.LogInformation(
            "AirtelMoney: Initiating payment for {Phone}, amount MWK {Amount}, ref {Ref}",
            request.Phone, request.Amount, request.Reference);

        try
        {
            // STUB: In production, first authenticate then POST to payment endpoint
            // await AuthenticateAsync();
            //
            // var payload = new {
            //     reference = request.Reference,
            //     subscriber = new { country = "MW", currency = "MWK", msisdn = request.Phone },
            //     transaction = new { amount = request.Amount, country = "MW", currency = "MWK",
            //                         id = Guid.NewGuid().ToString("N")[..10].ToUpper() }
            // };
            // var response = await _http.PostAsJsonAsync("merchant/v2/payments/", payload);
            // var result   = await response.Content.ReadFromJsonAsync<AirtelPaymentResponse>();

            // ── SIMULATED RESPONSE ────────────────────────────
            await Task.Delay(600); // simulate network latency

            var txRef = $"AM-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";

            _logger.LogInformation("AirtelMoney: Payment initiated. TxRef: {TxRef}", txRef);

            return new GatewayResult
            {
                Success              = true,
                TransactionReference = txRef,
                GatewayReference     = $"AIRTEL-{Guid.NewGuid():N}"[..20].ToUpper(),
                Status               = "Pending",
                Message              = $"Payment prompt sent to {request.Phone}. Awaiting PIN confirmation.",
                RawResponse          = JsonSerializer.Serialize(new
                {
                    status  = "SUCCESS",
                    data    = new { transaction = new { id = txRef, status = "TS" } },
                    message = "Prompt sent successfully"
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AirtelMoney: Payment initiation failed for {Ref}", request.Reference);
            return new GatewayResult
            {
                Success = false,
                Message = $"Airtel Money gateway error: {ex.Message}"
            };
        }
    }

    // ── QUERY STATUS ──────────────────────────────────────────
    public async Task<GatewayResult> QueryStatusAsync(string transactionReference)
    {
        _logger.LogInformation("AirtelMoney: Querying status for {TxRef}", transactionReference);

        try
        {
            // STUB: In production:
            // var response = await _http.GetAsync($"standard/v1/payments/{transactionReference}");
            // var result   = await response.Content.ReadFromJsonAsync<AirtelStatusResponse>();

            await Task.Delay(400);

            // Simulate a completed payment
            return new GatewayResult
            {
                Success              = true,
                TransactionReference = transactionReference,
                Status               = "Success",
                Message              = "Payment confirmed.",
                RawResponse          = JsonSerializer.Serialize(new
                {
                    status = "SUCCESS",
                    data   = new { transaction = new { id = transactionReference, status = "TS", message = "Paid" } }
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AirtelMoney: Status query failed for {TxRef}", transactionReference);
            return new GatewayResult { Success = false, Message = ex.Message };
        }
    }

    // ── CALLBACK ──────────────────────────────────────────────
    public async Task<GatewayCallbackResult> HandleCallbackAsync(string rawPayload)
    {
        _logger.LogInformation("AirtelMoney: Received callback payload");

        await Task.CompletedTask;

        try
        {
            // STUB: In production, parse and validate Airtel's callback JSON:
            // {
            //   "transaction": {
            //     "id": "TX123",
            //     "message": "Paid",
            //     "status_code": "TS",
            //     "airtel_money_id": "AM123"
            //   }
            // }

            var doc = JsonDocument.Parse(rawPayload);
            var tx  = doc.RootElement.GetProperty("transaction");

            var statusCode = tx.GetProperty("status_code").GetString();
            var txId       = tx.GetProperty("id").GetString();
            var amount     = tx.TryGetProperty("amount", out var amt) ? amt.GetDecimal() : 0m;

            return new GatewayCallbackResult
            {
                IsPaymentConfirmed   = statusCode == "TS", // TS = Transaction Successful
                TransactionReference = txId,
                Amount               = amount,
                RawPayload           = rawPayload
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AirtelMoney: Failed to parse callback payload");
            return new GatewayCallbackResult { IsPaymentConfirmed = false, RawPayload = rawPayload };
        }
    }

    // ── AUTH (for live integration) ───────────────────────────
    private async Task AuthenticateAsync()
    {
        if (_accessToken is not null && DateTime.UtcNow < _tokenExpiry)
            return; // token still valid

        var clientId     = _config["AirtelMoney:ClientId"];
        var clientSecret = _config["AirtelMoney:ClientSecret"];

        var response = await _http.PostAsJsonAsync("auth/oauth2/token", new
        {
            client_id     = clientId,
            client_secret = clientSecret,
            grant_type    = "client_credentials"
        });

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        _accessToken = result.GetProperty("access_token").GetString();
        var expiresIn = result.GetProperty("expires_in").GetInt32();
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

        _http.DefaultRequestHeaders.Remove("Authorization");
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
    }
}
