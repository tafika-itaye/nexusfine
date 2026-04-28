using System.Security.Claims;
using System.Text.Json;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Middleware;

/// <summary>
/// Records an AuditLog row for every mutating API call (POST/PUT/PATCH/DELETE)
/// against /api/*. Bulk lookups (GET) are deliberately skipped to keep the
/// log focused on state-changing actions.
/// </summary>
public class AuditLogMiddleware
{
    private static readonly HashSet<string> _mutatingMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "POST", "PUT", "PATCH", "DELETE"
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLogMiddleware> _logger;

    public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx, AppDbContext db)
    {
        var shouldLog = _mutatingMethods.Contains(ctx.Request.Method) &&
                        (ctx.Request.Path.StartsWithSegments("/api"));

        if (!shouldLog)
        {
            await _next(ctx);
            return;
        }

        await _next(ctx);

        try
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                       ?? ctx.User.FindFirstValue("sub")
                       ?? "anonymous";

            var ip = ctx.Connection.RemoteIpAddress?.ToString();

            var segments   = ctx.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var entityType = segments is { Length: >= 2 } ? segments[1] : "unknown";
            int entityId   = 0;

            // Case 1: ID is right there in the URL (PUT/PATCH/DELETE /api/fines/123)
            if (segments is { Length: >= 3 } && int.TryParse(segments[2], out var parsed))
                entityId = parsed;

            // Case 2: POST returned 201 Created — pull ID from the Location header
            // (ASP.NET sets it via CreatedAtAction, e.g. "/api/fines/123").
            if (entityId == 0 &&
                ctx.Response.StatusCode == StatusCodes.Status201Created &&
                ctx.Response.Headers.TryGetValue("Location", out var loc) &&
                loc.Count > 0)
            {
                var locParts = loc[0]?.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (locParts is { Length: > 0 } &&
                    int.TryParse(locParts[^1], out var locId))
                    entityId = locId;
            }

            var payload = new
            {
                method = ctx.Request.Method,
                path   = ctx.Request.Path.Value,
                query  = ctx.Request.QueryString.Value,
                status = ctx.Response.StatusCode
            };

            db.AuditLogs.Add(new AuditLog
            {
                EntityType = entityType,
                EntityId   = entityId,
                Action     = ctx.Request.Method,
                NewValues  = JsonSerializer.Serialize(payload),
                UserId     = userId,
                IpAddress  = ip,
                Timestamp  = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log entry.");
        }
    }
}

public static class AuditLogMiddlewareExtensions
{
    public static IApplicationBuilder UseNexusFineAuditLog(this IApplicationBuilder app) =>
        app.UseMiddleware<AuditLogMiddleware>();
}
