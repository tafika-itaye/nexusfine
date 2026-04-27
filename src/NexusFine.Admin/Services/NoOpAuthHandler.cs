using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace NexusFine.Admin.Services;

public class NoOpAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public NoOpAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder) { }

    // Returns a permissive principal so HTTP-level [Authorize(Roles=...)] always passes.
    // Real auth still happens in the Blazor circuit via JwtAuthStateProvider →
    // AuthorizeRouteView checks the in-memory JWT and redirects unauth'd users to /login.
    // The NoOp scheme exists only to satisfy ASP.NET Core's IAuthenticationService
    // requirement when [Authorize] attributes are promoted to endpoint metadata by
    // MapRazorComponents.
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, "circuit"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "Supervisor"),
            },
            authenticationType: "NoOp");
        var principal = new ClaimsPrincipal(identity);
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(principal, "NoOp")));
    }
}
