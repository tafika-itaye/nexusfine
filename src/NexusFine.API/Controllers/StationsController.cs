using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supervisor")]
public class StationsController : ControllerBase
{
    private readonly AppDbContext _db;
    public StationsController(AppDbContext db) => _db = db;

    // GET api/stations
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? zone, [FromQuery] int? departmentId, [FromQuery] bool? activeOnly)
    {
        IQueryable<Station> q = _db.Stations.Include(s => s.Department);

        if (!string.IsNullOrWhiteSpace(zone))   q = q.Where(s => s.Zone == zone);
        if (departmentId.HasValue)              q = q.Where(s => s.DepartmentId == departmentId.Value);
        if (activeOnly == true)                 q = q.Where(s => s.IsActive);

        var list = await q.OrderBy(s => s.Zone).ThenBy(s => s.Name)
            .Select(s => new
            {
                s.Id, s.Code, s.Name, s.DepartmentId,
                DepartmentName = s.Department.Name,
                s.Zone, s.PhysicalAddress, s.ContactPhone,
                s.OfficerInChargeBadge,
                s.StationServerEndpoint,
                s.LastSyncAt, s.ConsecutiveFailedSyncs,
                s.Lat, s.Lng, s.IsActive,
                OfficerCount    = s.Officers.Count,
                PatrolPostCount = s.PatrolPosts.Count,
                DeviceCount     = s.Devices.Count(d => d.Status != DeviceStatus.Retired)
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET api/stations/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Stations
            .Include(x => x.Department)
            .Include(x => x.PatrolPosts)
            .Include(x => x.Officers)
            .Include(x => x.Devices)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (s is null) return NotFound();

        return Ok(new
        {
            s.Id, s.Code, s.Name, s.DepartmentId,
            DepartmentName = s.Department.Name,
            s.Zone, s.PhysicalAddress, s.ContactPhone,
            s.OfficerInChargeBadge,
            s.StationServerEndpoint, s.StationServerPublicKey,
            s.LastSyncAt, s.ConsecutiveFailedSyncs,
            s.Lat, s.Lng, s.IsActive,
            PatrolPosts = s.PatrolPosts.OrderBy(p => p.Code).Select(p => new
            {
                p.Id, p.Code, p.Name, p.IsActive
            }),
            Officers = s.Officers.OrderBy(o => o.BadgeNumber).Select(o => new
            {
                o.Id, o.BadgeNumber, o.FullName, o.Rank, o.Status, o.IsActive
            }),
            Devices = s.Devices.OrderBy(d => d.Serial).Select(d => new
            {
                d.Id, d.Serial, d.Model, d.Status,
                d.LastSeenAt, d.LastSyncAt,
                CurrentOfficerBadge = d.CurrentOfficer == null ? null : d.CurrentOfficer.BadgeNumber
            })
        });
    }

    // POST api/stations
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] StationUpsert req)
    {
        if (string.IsNullOrWhiteSpace(req.Code) || string.IsNullOrWhiteSpace(req.Name))
            return BadRequest(new { message = "Code and Name are required." });

        if (await _db.Stations.AnyAsync(s => s.Code == req.Code))
            return Conflict(new { message = $"Station code '{req.Code}' is already in use." });

        var dept = await _db.Departments.FindAsync(req.DepartmentId);
        if (dept is null)
            return BadRequest(new { message = $"Department {req.DepartmentId} not found." });

        var s = new Station
        {
            Code                  = req.Code.Trim(),
            Name                  = req.Name.Trim(),
            DepartmentId          = req.DepartmentId,
            Zone                  = string.IsNullOrWhiteSpace(req.Zone) ? dept.Zone : req.Zone.Trim(),
            PhysicalAddress       = req.PhysicalAddress?.Trim() ?? "",
            ContactPhone          = req.ContactPhone?.Trim(),
            OfficerInChargeBadge  = req.OfficerInChargeBadge?.Trim(),
            StationServerEndpoint = req.StationServerEndpoint?.Trim(),
            Lat                   = req.Lat,
            Lng                   = req.Lng,
            IsActive              = req.IsActive
        };
        _db.Stations.Add(s);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = s.Id }, new { s.Id, s.Code });
    }

    // PUT api/stations/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] StationUpsert req)
    {
        var s = await _db.Stations.FindAsync(id);
        if (s is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(req.Code) && req.Code != s.Code &&
            await _db.Stations.AnyAsync(x => x.Code == req.Code && x.Id != id))
            return Conflict(new { message = $"Station code '{req.Code}' is already in use." });

        s.Code                  = req.Code?.Trim() ?? s.Code;
        s.Name                  = req.Name?.Trim() ?? s.Name;
        s.DepartmentId          = req.DepartmentId == 0 ? s.DepartmentId : req.DepartmentId;
        s.Zone                  = string.IsNullOrWhiteSpace(req.Zone) ? s.Zone : req.Zone.Trim();
        s.PhysicalAddress       = req.PhysicalAddress?.Trim() ?? s.PhysicalAddress;
        s.ContactPhone          = req.ContactPhone?.Trim();
        s.OfficerInChargeBadge  = req.OfficerInChargeBadge?.Trim();
        s.StationServerEndpoint = req.StationServerEndpoint?.Trim();
        s.Lat                   = req.Lat ?? s.Lat;
        s.Lng                   = req.Lng ?? s.Lng;
        s.IsActive              = req.IsActive;
        s.UpdatedAt             = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { s.Id, s.Code });
    }

    // DELETE api/stations/{id}  (soft — sets IsActive=false; refuses if officers/devices still attached)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var s = await _db.Stations
            .Include(x => x.Officers)
            .Include(x => x.Devices)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (s is null) return NotFound();

        var attachedOfficers = s.Officers.Count(o => o.IsActive);
        var attachedDevices  = s.Devices.Count(d => d.Status != DeviceStatus.Retired && d.Status != DeviceStatus.Revoked);

        if (attachedOfficers > 0 || attachedDevices > 0)
            return Conflict(new
            {
                message = "Reassign officers and devices before deactivating this station.",
                attachedOfficers, attachedDevices
            });

        s.IsActive  = false;
        s.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record StationUpsert(
    string  Code,
    string  Name,
    int     DepartmentId,
    string? Zone,
    string? PhysicalAddress,
    string? ContactPhone,
    string? OfficerInChargeBadge,
    string? StationServerEndpoint,
    decimal? Lat,
    decimal? Lng,
    bool    IsActive
);
