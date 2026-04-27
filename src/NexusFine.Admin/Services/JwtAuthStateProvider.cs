using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace NexusFine.Admin.Services;

/// <summary>
/// Circuit-scoped JWT-backed authentication state provider.
///
/// The token is held in memory for the lifetime of the SignalR circuit
/// (i.e. the browser tab + connection). On disconnect or reload the user
/// is bounced to /login. We do NOT persist the token to ProtectedSessionStorage
/// for the demo — if/when post-demo we want refresh-on-reload, swap in a
/// PersistentComponentState rehydration step.
///
/// Roles are extracted from the JWT's role claims so [Authorize(Roles="Admin")]
/// works exactly the same way as on the API side.
/// </summary>
public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal AnonymousUser =
        new(new ClaimsIdentity());

    private readonly JwtSecurityTokenHandler _handler = new();
    private string?         _accessToken;
    private DateTime?       _expiresAt;
    private ClaimsPrincipal _principal = AnonymousUser;

    public string? AccessToken     => _accessToken;
    public DateTime? ExpiresAt     => _expiresAt;
    public bool      IsAuthenticated => _principal.Identity?.IsAuthenticated == true;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // If the in-memory token has expired, treat as anonymous.
        if (_expiresAt.HasValue && _expiresAt.Value <= DateTime.UtcNow)
        {
            _accessToken = null;
            _expiresAt   = null;
            _principal   = AnonymousUser;
        }
        return Task.FromResult(new AuthenticationState(_principal));
    }

    public Task SignInAsync(string accessToken, DateTime expiresAt)
    {
        _accessToken = accessToken;
        _expiresAt   = expiresAt;

        var token = _handler.ReadJwtToken(accessToken);
        var claims = token.Claims.ToList();

        // The "role" claim from the API side is JwtRegisteredClaimNames.Sub-style;
        // the Microsoft handler usually stores it as ClaimTypes.Role already, but
        // when reading back via ReadJwtToken the type can come through as the short
        // form ("role"). Normalise so AuthorizeView Roles="..." matches.
        claims = claims
            .Select(c => c.Type == "role" ? new Claim(ClaimTypes.Role, c.Value) : c)
            .ToList();

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "jwt",
            nameType:           ClaimTypes.Name,
            roleType:           ClaimTypes.Role);

        _principal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));
        return Task.CompletedTask;
    }

    public Task SignOutAsync()
    {
        _accessToken = null;
        _expiresAt   = null;
        _principal   = AnonymousUser;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));
        return Task.CompletedTask;
    }
}
