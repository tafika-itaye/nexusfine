namespace NexusFine.Infrastructure.Services;

// ── SHARED GATEWAY INTERFACE ──────────────────────────────────
// Both Airtel Money and TNM Mpamba implement this contract.
// When real gateway credentials are available, only the
// implementation changes — the rest of the system stays the same.

public interface IPaymentGateway
{
    /// <summary>
    /// Initiate a payment request to the gateway.
    /// Returns a GatewayResult with a transaction reference.
    /// </summary>
    Task<GatewayResult> InitiateAsync(GatewayRequest request);

    /// <summary>
    /// Query the gateway for the current status of a transaction.
    /// </summary>
    Task<GatewayResult> QueryStatusAsync(string transactionReference);

    /// <summary>
    /// Process an inbound callback payload from the gateway.
    /// Returns true if payment is confirmed.
    /// </summary>
    Task<GatewayCallbackResult> HandleCallbackAsync(string rawPayload);
}

// ── REQUEST ───────────────────────────────────────────────────
public class GatewayRequest
{
    public string  Phone           { get; set; } = string.Empty;
    public decimal Amount          { get; set; }
    public string  Currency        { get; set; } = "MWK";
    public string  Reference       { get; set; } = string.Empty; // NXF fine ref
    public string  Description     { get; set; } = string.Empty;
    public string  CallbackUrl     { get; set; } = string.Empty;
}

// ── RESULT ────────────────────────────────────────────────────
public class GatewayResult
{
    public bool    Success              { get; set; }
    public string? TransactionReference { get; set; }
    public string? GatewayReference    { get; set; } // gateway's own ID
    public string? Status              { get; set; } // Pending, Success, Failed
    public string? Message             { get; set; }
    public string? RawResponse         { get; set; } // raw JSON for audit
}

public class GatewayCallbackResult
{
    public bool    IsPaymentConfirmed  { get; set; }
    public string? TransactionReference{ get; set; }
    public string? GatewayReference   { get; set; }
    public decimal Amount             { get; set; }
    public string? RawPayload         { get; set; }
}
