using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NexusFine.Core.Entities;

namespace NexusFine.Infrastructure.Auth;

public class JwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
        if (string.IsNullOrWhiteSpace(_settings.Secret) || _settings.Secret.Length < 32)
            throw new InvalidOperationException(
                "Jwt:Secret must be configured and at least 32 characters long.");
    }

    public AuthTokenResult Issue(AppUser user)
    {
        var now         = DateTime.UtcNow;
        var accessExp   = now.AddMinutes(_settings.AccessTokenMinutes);
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds       = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new("fullName", user.FullName),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        if (user.OfficerId.HasValue)
            claims.Add(new Claim("officerId", user.OfficerId.Value.ToString()));

        foreach (var role in (user.Roles ?? string.Empty)
                             .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer:             _settings.Issuer,
            audience:           _settings.Audience,
            claims:             claims,
            notBefore:          now,
            expires:            accessExp,
            signingCredentials: creds);

        var accessToken  = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        return new AuthTokenResult(
            accessToken,
            accessExp,
            refreshToken,
            now.AddDays(_settings.RefreshTokenDays));
    }

    public static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public TokenValidationParameters BuildValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = _settings.Issuer,
            ValidAudience            = _settings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
            ClockSkew                = TimeSpan.FromSeconds(30)
        };
    }
}

public record AuthTokenResult(
    string   AccessToken,
    DateTime AccessTokenExpiresAt,
    string   RefreshToken,
    DateTime RefreshTokenExpiresAt);
