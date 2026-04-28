using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supervisor")]
public class PatrolPostsController : ControllerBase
{
    private readonly AppDbContext _db;
    public PatrolPostsController(AppDbContext db) => _db = db;

    // GET api/patrolposts?stationId=&activeOnly=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? stationId, [FromQuery] bool? activeOnly)
    {
        IQueryable<PatrolPost> q = _db.PatrolPosts.Include(p => p.Station);
        if (stationId.HasValue) q = q.Where(p => p.StationId == stationId.Value);
        if (activeOnly == true) q = q.Where(p => p.IsActive);

        var list = await q.OrderBy(p => p.Code)
            .Select(p => new
            {
                p.Id, p.Code, p.Name, p.StationId,
                StationCode = p.Station.Code,
                StationName = p.Station.Name,
                Zone        = p.Station.Zone,
                p.Lat, p.Lng, p.Notes, p.IsActive
            })
            .ToListAsync();

        return Ok(list);
    }

    // POST api/patrolposts
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] PatrolPostUpsert req)
    {
        if (string.IsNullOrWhiteSpace(req.Code) || string.IsNullOrWhiteSpace(req.Name))
            return BadRequest(new { message = "Code and Name are required." });

        if (await _db.PatrolPosts.AnyAsync(p => p.Code == req.Code))
            return Conflict(new { message = $"Patrol post code '{req.Code}' is already in use." });

        if (!await _db.Stations.AnyAsync(s => s.Id == req.StationId))
            return BadRequest(new { message = $"Station {req.StationId} not found." });

        var p = new PatrolPost
        {
            Code      = req.Code.Trim(),
            Name      = req.Name.Trim(),
            StationId = req.StationId,
            Lat       = req.Lat,
            Lng       = req.Lng,
            Notes     = req.Notes?.Trim(),
            IsActive  = req.IsActive
        };
        _db.PatrolPosts.Add(p);
        await _db.SaveChangesAsync();
        return Created($"api/patrolposts/{p.Id}", new { p.Id, p.Code });
    }

    // PUT api/patrolposts/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] PatrolPostUpsert req)
    {
        var p = await _db.PatrolPosts.FindAsync(id);
        if (p is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(req.Code) && req.Code != p.Code &&
            await _db.PatrolPosts.AnyAsync(x => x.Code == req.Code && x.Id != id))
            return Conflict(new { message = $"Patrol post code '{req.Code}' is already in use." });

        p.Code      = req.Code?.Trim() ?? p.Code;
        p.Name      = req.Name?.Trim() ?? p.Name;
        p.StationId = req.StationId == 0 ? p.StationId : req.StationId;
        p.Lat       = req.Lat ?? p.Lat;
        p.Lng       = req.Lng ?? p.Lng;
        p.Notes     = req.Notes?.Trim();
        p.IsActive  = req.IsActive;
        p.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { p.Id, p.Code });
    }

    // DELETE api/patrolposts/{id}  (soft)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var p = await _db.PatrolPosts.FindAsync(id);
        if (p is null) return NotFound();
        p.IsActive  = false;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record PatrolPostUpsert(
    string  Code,
    string  Name,
    int     StationId,
    decimal? Lat,
    decimal? Lng,
    string? Notes,
    bool    IsActive
);
