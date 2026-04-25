using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Auth;
using NexusFine.Infrastructure.Data;
using System.Security.Claims;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext     _db;
    private readonly PasswordHasher   _hasher;
    private readonly JwtTokenService  _jwt;

    public AuthController(AppDbContext db, PasswordHasher hasher, JwtTokenService jwt)
    {
        _db     = db;
        _hasher = hasher;
        _jwt    = jwt;
    }

    // POST api/auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UserName) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Username and password are required." });

        var user = await _db.AppUsers
            .Include(u => u.Officer)
            .FirstOrDefaultAsync(u => u.UserName == req.UserName && u.IsActive);

        if (user is null || !_hasher.Verify(req.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized(new { message = "Invalid credentials." });

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt   = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var tokens = _jwt.Issue(user);
        return Ok(new
        {
            tokens.AccessToken,
            tokens.AccessTokenExpiresAt,
            tokens.RefreshToken,
            tokens.RefreshTokenExpiresAt,
            user = new
            {
                user.Id,
                user.UserName,
                user.FullName,
                user.Email,
                roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                officerId = user.OfficerId
            }
        });
    }

    // POST api/auth/register  (Admin only)
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UserName) ||
            string.IsNullOrWhiteSpace(req.Password) ||
            string.IsNullOrWhiteSpace(req.FullName) ||
            string.IsNullOrWhiteSpace(req.Roles))
            return BadRequest(new { message = "UserName, Password, FullName and Roles are required." });

        if (await _db.AppUsers.AnyAsync(u => u.UserName == req.UserName))
            return Conflict(new { message = "A user with this username already exists." });

        var (hash, salt) = _hasher.Hash(req.Password);
        var user = new AppUser
        {
            UserName     = req.UserName.Trim(),
            Email        = req.Email?.Trim() ?? string.Empty,
            FullName     = req.FullName.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Roles        = req.Roles.Trim(),
            OfficerId    = req.OfficerId,
            IsActive     = true
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Me), new { }, new
        {
            user.Id,
            user.UserName,
            user.FullName,
            user.Email,
            user.Roles,
            user.OfficerId
        });
    }

    // GET api/auth/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idStr, out var id))
            return Unauthorized();

        var user = await _db.AppUsers
            .Include(u => u.Officer)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.FullName,
            user.Email,
            roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            officer = user.Officer is null ? null : new
            {
                user.Officer.Id,
                user.Officer.BadgeNumber,
                user.Officer.FullName,
                user.Officer.Rank,
                user.Officer.DepartmentId
            }
        });
    }
}

public record LoginRequest(string UserName, string Password);
public record RegisterRequest(
    string  UserName,
    string  Password,
    string  FullName,
    string? Email,
    string  Roles,      // comma-separated
    int?    OfficerId);
