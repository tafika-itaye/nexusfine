namespace NexusFine.Infrastructure.Auth;

/// <summary>
/// Bound from the "Jwt" section of appsettings.json.
/// </summary>
public class JwtSettings
{
    public string Issuer   { get; set; } = "NexusFine";
    public string Audience { get; set; } = "NexusFine.Clients";
    public string Secret   { get; set; } = string.Empty;    // >= 32 chars in prod
    public int    AccessTokenMinutes  { get; set; } = 120;
    public int    RefreshTokenDays    { get; set; } = 14;
}
