using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supervisor")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuditLogsController(AppDbContext db) => _db = db;

    // GET api/auditlogs?entityType=Fine&action=Created&from=2026-04-01&to=2026-04-30&page=1&pageSize=50
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string?   entityType,
        [FromQuery] string?   action,
        [FromQuery] int?      entityId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int       page     = 1,
        [FromQuery] int       pageSize = 50)
    {
        if (page     < 1)   page     = 1;
        if (pageSize < 1)   pageSize = 50;
        if (pageSize > 200) pageSize = 200;

        var q = _db.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
            q = q.Where(a => a.EntityType == entityType);

        if (!string.IsNullOrWhiteSpace(action))
            q = q.Where(a => a.Action == action);

        if (entityId.HasValue)
            q = q.Where(a => a.EntityId == entityId.Value);

        if (from.HasValue)
            q = q.Where(a => a.Timestamp >= from.Value);

        if (to.HasValue)
            q = q.Where(a => a.Timestamp <= to.Value);

        var total = await q.CountAsync();
        var rows  = await q
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.Id,
                a.EntityType,
                a.EntityId,
                a.Action,
                a.OldValues,
                a.NewValues,
                a.UserId,
                a.IpAddress,
                a.Timestamp
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, data = rows });
    }

    // GET api/auditlogs/entitytypes  (distinct list — populates filter dropdown)
    [HttpGet("entitytypes")]
    public async Task<IActionResult> GetEntityTypes()
    {
        var types = await _db.AuditLogs
            .Select(a => a.EntityType)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
        return Ok(types);
    }

    // GET api/auditlogs/actions  (distinct list — populates filter dropdown)
    [HttpGet("actions")]
    public async Task<IActionResult> GetActions()
    {
        var actions = await _db.AuditLogs
            .Select(a => a.Action)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
        return Ok(actions);
    }
}
