using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supervisor")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public DepartmentsController(AppDbContext db) => _db = db;

    // GET api/departments?region=&q=&activeOnly=
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? region,
        [FromQuery] string? q,
        [FromQuery] bool? activeOnly)
    {
        IQueryable<Department> query = _db.Departments;
        if (!string.IsNullOrWhiteSpace(region)) query = query.Where(d => d.Region == region);
        if (activeOnly == true)                 query = query.Where(d => d.IsActive);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim();
            query = query.Where(d => d.Name.Contains(s) || d.Zone.Contains(s));
        }

        var depts = await query
            .OrderBy(d => d.Region).ThenBy(d => d.Zone).ThenBy(d => d.Name)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Zone,
                d.Region,
                d.HeadOfficerBadge,
                d.IsActive,
                OfficerCount       = d.Officers.Count,
                ActiveOfficerCount = d.Officers.Count(o => o.IsActive && o.Status == OfficerStatus.Active),
                StationCount       = d.Stations.Count(s => s.IsActive),
                FineCount          = d.Fines.Count,
                FineRevenue        = d.Fines
                    .Where(f => f.Status == FineStatus.Paid)
                    .Sum(f => (decimal?)f.Amount) ?? 0m
            })
            .ToListAsync();

        return Ok(depts);
    }

    // GET api/departments/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await _db.Departments
            .Include(d => d.Officers)
            .Include(d => d.Stations)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dept is null) return NotFound();

        return Ok(new
        {
            dept.Id,
            dept.Name,
            dept.Zone,
            dept.Region,
            dept.HeadOfficerBadge,
            dept.IsActive,
            Officers = dept.Officers
                .OrderBy(o => o.BadgeNumber)
                .Select(o => new { o.Id, o.BadgeNumber, o.FullName, o.Rank, o.Status, o.IsActive }),
            Stations = dept.Stations
                .OrderBy(s => s.Code)
                .Select(s => new { s.Id, s.Code, s.Name, s.Zone, s.IsActive })
        });
    }

    // POST api/departments
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] DepartmentUpsert req)
    {
        var err = await Validate(req, null);
        if (err is not null) return err;

        var d = new Department
        {
            Name             = req.Name.Trim(),
            Zone             = req.Zone.Trim(),
            Region           = req.Region.Trim(),
            HeadOfficerBadge = req.HeadOfficerBadge?.Trim(),
            IsActive         = req.IsActive
        };
        _db.Departments.Add(d);
        await _db.SaveChangesAsync();
        return Created($"api/departments/{d.Id}", new { d.Id, d.Name });
    }

    // PUT api/departments/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentUpsert req)
    {
        var d = await _db.Departments.FindAsync(id);
        if (d is null) return NotFound();

        var err = await Validate(req, id);
        if (err is not null) return err;

        d.Name             = req.Name.Trim();
        d.Zone             = req.Zone.Trim();
        d.Region           = req.Region.Trim();
        d.HeadOfficerBadge = req.HeadOfficerBadge?.Trim();
        d.IsActive         = req.IsActive;

        await _db.SaveChangesAsync();
        return Ok(new { d.Id, d.Name });
    }

    // DELETE api/departments/{id}  — soft (refuses if officers/stations/fines attached)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var d = await _db.Departments
            .Include(x => x.Officers)
            .Include(x => x.Stations)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (d is null) return NotFound();

        var attachedOfficers = d.Officers.Count(o => o.IsActive);
        var attachedStations = d.Stations.Count(s => s.IsActive);
        var attachedFines    = await _db.Fines.CountAsync(f => f.DepartmentId == id);

        if (attachedOfficers > 0 || attachedStations > 0)
            return Conflict(new
            {
                message = "Reassign active officers and stations before deactivating this department.",
                attachedOfficers, attachedStations, attachedFines
            });

        d.IsActive = false;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<IActionResult?> Validate(DepartmentUpsert req, int? id)
    {
        if (string.IsNullOrWhiteSpace(req.Name) ||
            string.IsNullOrWhiteSpace(req.Zone) ||
            string.IsNullOrWhiteSpace(req.Region))
            return BadRequest(new { message = "Name, Zone (district) and Region are required." });

        if (await _db.Departments.AnyAsync(d => d.Name == req.Name && (id == null || d.Id != id)))
            return Conflict(new { message = $"Department '{req.Name}' already exists." });

        return null;
    }
}

public record DepartmentUpsert(
    string  Name,
    string  Zone,
    string  Region,
    string? HeadOfficerBadge,
    bool    IsActive
);
