using Microsoft.Extensions.Logging;
using NexusFine.Core.Entities;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// Deterministic fake gateway for the Minister demo and automated tests.
/// Routes to this implementation when ApiSettings:PaymentMode=Simulated.
///
/// Behaviour:
///   • InitiateAsync always succeeds immediately and returns a SIM- reference.
///   • QueryStatusAsync echoes a Success status for any known reference.
///   • HandleCallbackAsync parses a minimal JSON payload of the form:
///         { "reference": "SIM-...", "amount": 50000, "confirmed": true }
///
/// In production, flip the config to PaymentMode=Live and the factory will
/// route to AirtelMoneyService / MpambaService against real endpoints.
/// </summary>
public class SimulatedPaymentGateway : IPaymentGateway
{
    private readonly ILogger<SimulatedPaymentGateway> _logger;

    public SimulatedPaymentGateway(ILogger<SimulatedPaymentGateway> logger)
    {
        _logger = logger;
    }

    public Task<GatewayResult> InitiateAsync(GatewayRequest request)
    {
        var txn = $"SIM-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}".Substring(0, 38);
        _logger.LogInformation(
            "[SIMULATED] Payment initiated. Phone={Phone} Amount={Amount} Ref={Reference} Txn={Txn}",
            request.Phone, request.Amount, request.Reference, txn);

        return Task.FromResult(new GatewayResult
        {
            Success              = true,
            TransactionReference = txn,
            GatewayReference     = txn,
            Status               = "Pending",
            Message              = "Simulated payment initiated. Citizen would now authorize on handset.",
            RawResponse          = $"{{\"simulated\":true,\"txn\":\"{txn}\"}}"
        });
    }

    public Task<GatewayResult> QueryStatusAsync(string transactionReference)
    {
        _logger.LogInformation("[SIMULATED] Status query for {Txn}", transactionReference);

        return Task.FromResult(new GatewayResult
        {
            Success              = true,
            TransactionReference = transactionReference,
            GatewayReference     = transactionReference,
            Status               = "Success",
            Message              = "Simulated payment confirmed.",
            RawResponse          = $"{{\"simulated\":true,\"status\":\"Success\"}}"
        });
    }

    public Task<GatewayCallbackResult> HandleCallbackAsync(string rawPayload)
    {
        _logger.LogInformation("[SIMULATED] Callback received. Payload={Payload}", rawPayload);

        // Very small JSON parser — only what the fake endpoint emits.
        var reference = ExtractString(rawPayload, "reference");
        var amountStr = ExtractString(rawPayload, "amount");
        decimal.TryParse(amountStr, out var amount);

        var confirmed = !rawPayload.Contains("\"confirmed\":false",
                         StringComparison.OrdinalIgnoreCase);

        return Task.FromResult(new GatewayCallbackResult
        {
            IsPaymentConfirmed   = confirmed,
            TransactionReference = reference ?? string.Empty,
            GatewayReference     = reference,
            Amount               = amount,
            RawPayload           = rawPayload
        });
    }

    private static string? ExtractString(string json, string key)
    {
        var needle = $"\"{key}\":";
        var idx = json.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return null;
        var start = idx + needle.Length;
        while (start < json.Length && (json[start] == ' ' || json[start] == '"')) start++;
        var end = start;
        while (end < json.Length && json[end] != '"' && json[end] != ',' && json[end] != '}') end++;
        return json.Substring(start, end - start).Trim();
    }
}
