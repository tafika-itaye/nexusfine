using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Auth;
using Xunit;

namespace NexusFine.Tests;

public class JwtTokenServiceTests
{
    private readonly JwtSettings _settings = new()
    {
        Issuer             = "NexusFine",
        Audience           = "NexusFine.Clients",
        Secret             = "TEST-SECRET-AT-LEAST-32-CHARACTERS-LONG-2026",
        AccessTokenMinutes = 60,
        RefreshTokenDays   = 14
    };

    private JwtTokenService Service() =>
        new(Options.Create(_settings));

    [Fact]
    public void Issue_ReturnsAccessAndRefreshTokens()
    {
        var svc    = Service();
        var user   = new AppUser { Id = 1, UserName = "admin", Email = "a@b.mw", FullName = "A B", Roles = "Admin" };
        var result = svc.Issue(user);

        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.True(result.AccessTokenExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void Issue_EmbedsRoleClaims()
    {
        var svc  = Service();
        var user = new AppUser { Id = 2, UserName = "sup", FullName = "Sup", Roles = "Supervisor,Admin" };
        var jwt  = svc.Issue(user);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt.AccessToken);
        var roles = token.Claims
                         .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                         .Select(c => c.Value)
                         .ToArray();

        Assert.Contains("Supervisor", roles);
        Assert.Contains("Admin", roles);
    }

    [Fact]
    public void Issue_ShortSecret_Throws()
    {
        var bad = new JwtSettings { Secret = "too-short" };
        Assert.Throws<InvalidOperationException>(() =>
            new JwtTokenService(Options.Create(bad)));
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsBase64()
    {
        var token = JwtTokenService.GenerateRefreshToken();
        Assert.False(string.IsNullOrWhiteSpace(token));
        var bytes = Convert.FromBase64String(token);
        Assert.Equal(64, bytes.Length);
    }
}
