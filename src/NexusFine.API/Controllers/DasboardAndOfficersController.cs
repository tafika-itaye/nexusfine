using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

// ── DASHBOARD ─────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Supervisor,Admin")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    // GET api/dashboard/kpis
    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis()
    {
        var today     = DateTime.UtcNow.Date;
        var monthStart= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var issuedToday    = await _db.Fines.CountAsync(f => f.IssuedAt >= today);
        var revenueMonth   = await _db.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CompletedAt >= monthStart)
            .SumAsync(p => (decimal?)p.Amount) ?? 0;
        var totalMonth     = await _db.Fines
            .Where(f => f.IssuedAt >= monthStart)
            .SumAsync(f => (decimal?)f.Amount) ?? 0;
        var collectionRate = totalMonth > 0
            ? Math.Round(revenueMonth / totalMonth * 100, 1)
            : 0;
        var officersOnDuty = await _db.Officers.CountAsync(o =>
            o.IsActive && o.Status == OfficerStatus.Active);

        return Ok(new
        {
            issuedToday,
            revenueMonth,
            collectionRate,
            officersOnDuty,
            officersTotal = await _db.Officers.CountAsync(o => o.IsActive)
        });
    }

    // GET api/dashboard/trend?period=7d
    [HttpGet("trend")]
    public async Task<IActionResult> GetTrend([FromQuery] string period = "7d")
    {
        var days = period switch { "14d" => 14, "30d" => 30, _ => 7 };
        var from = DateTime.UtcNow.Date.AddDays(-days + 1);

        var fines = await _db.Fines
            .Where(f => f.IssuedAt >= from)
            .GroupBy(f => f.IssuedAt.Date)
            .Select(g => new
            {
                Date      = g.Key,
                Issued    = g.Count(),
                Collected = g.Count(f => f.Status == FineStatus.Paid)
            })
            .OrderBy(g => g.Date)
            .ToListAsync();

        return Ok(new
        {
            labels    = fines.Select(f => f.Date.ToString("dd MMM")),
            issued    = fines.Select(f => f.Issued),
            collected = fines.Select(f => f.Collected)
        });
    }

    // GET api/dashboard/reconciliation
    [HttpGet("reconciliation")]
    public async Task<IActionResult> GetReconciliation(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string?   zone)
    {
        var start = from ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var end   = to   ?? DateTime.UtcNow;

        var query = _db.Fines
            .Include(f => f.Department)
            .Where(f => f.IssuedAt >= start && f.IssuedAt <= end);

        if (!string.IsNullOrWhiteSpace(zone))
            query = query.Where(f => f.Department.Zone == zone);

        var fines = await query.ToListAsync();

        var totalIssued      = fines.Sum(f => f.Amount);
        var totalCollected   = fines.Where(f => f.Status == FineStatus.Paid).Sum(f => f.Amount);
        var totalOutstanding = fines.Where(f => f.Status != FineStatus.Paid && f.Status != FineStatus.Cancelled).Sum(f => f.Amount);
        var totalOverdue     = fines.Where(f => f.Status == FineStatus.Overdue).Sum(f => f.Amount);

        var byZone = fines
            .GroupBy(f => f.Department.Zone)
            .Select(g => new
            {
                Zone        = g.Key,
                Issued      = g.Count(),
                Collected   = g.Count(f => f.Status == FineStatus.Paid),
                AmountIssued    = g.Sum(f => f.Amount),
                AmountCollected = g.Where(f => f.Status == FineStatus.Paid).Sum(f => f.Amount),
                Rate        = g.Sum(f => f.Amount) > 0
                    ? Math.Round(g.Where(f => f.Status == FineStatus.Paid).Sum(f => f.Amount)
                        / g.Sum(f => f.Amount) * 100, 1) : 0
            });

        return Ok(new { totalIssued, totalCollected, totalOutstanding, totalOverdue, byZone });
    }

    // GET api/dashboard/topoffences
    [HttpGet("topoffences")]
    public async Task<IActionResult> GetTopOffences([FromQuery] int top = 10)
    {
        var offences = await _db.Fines
            .Include(f => f.OffenceCode)
            .GroupBy(f => new { f.OffenceCodeId, f.OffenceCode.Name })
            .Select(g => new
            {
                g.Key.Name,
                Count  = g.Count(),
                Amount = g.Sum(f => f.Amount)
            })
            .OrderByDescending(g => g.Count)
            .Take(top)
            .ToListAsync();

        return Ok(offences);
    }

    // GET api/dashboard/recent?limit=8
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentFines([FromQuery] int limit = 8)
    {
        var fines = await _db.Fines
            .Include(f => f.OffenceCode)
            .OrderByDescending(f => f.IssuedAt)
            .Take(limit)
            .Select(f => new
            {
                f.ReferenceNumber,
                f.PlateNumber,
                f.Status,
                f.Amount,
                f.IssuedAt,
                Offence = f.OffenceCode.Name
            })
            .ToListAsync();

        return Ok(fines);
    }
}

// ── OFFICERS ──────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OfficersController : ControllerBase
{
    private readonly AppDbContext _db;
    public OfficersController(AppDbContext db) => _db = db;

    // GET api/officers
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? departmentId, [FromQuery] string? zone)
    {
        var query = _db.Officers
            .Include(o => o.Department)
            .Where(o => o.IsActive)
            .AsQueryable();

        if (departmentId.HasValue)
            query = query.Where(o => o.DepartmentId == departmentId.Value);

        if (!string.IsNullOrWhiteSpace(zone))
            query = query.Where(o => o.Department.Zone == zone);

        var officers = await query
            .OrderBy(o => o.LastName)
            .ToListAsync();

        return Ok(officers.Select(MapToDto));
    }

    // GET api/officers/performance?date=today
    [HttpGet("performance")]
    public async Task<IActionResult> GetPerformance([FromQuery] string date = "today")
    {
        var targetDate = date == "today"
            ? DateTime.UtcNow.Date
            : DateTime.TryParse(date, out var d) ? d.Date : DateTime.UtcNow.Date;

        var officers = await _db.Officers
            .Include(o => o.Department)
            .Include(o => o.Fines.Where(f => f.IssuedAt.Date == targetDate))
            .Where(o => o.IsActive)
            .ToListAsync();

        var result = officers.Select(o => new
        {
            o.Id,
            o.BadgeNumber,
            o.FullName,
            o.Rank,
            o.Status,
            Zone             = o.Department.Zone,
            o.LastKnownLocation,
            o.LastSyncAt,
            o.DeviceId,
            FinesIssued      = o.Fines.Count,
            FinesCollected   = o.Fines.Count(f => f.Status == FineStatus.Paid),
            AmountIssued     = o.Fines.Sum(f => f.Amount),
            AmountCollected  = o.Fines.Where(f => f.Status == FineStatus.Paid).Sum(f => f.Amount),
            CollectionRate   = o.Fines.Count > 0
                ? Math.Round((double)o.Fines.Count(f => f.Status == FineStatus.Paid)
                    / o.Fines.Count * 100, 1) : 0
        });

        return Ok(result.OrderByDescending(o => o.FinesIssued));
    }

    // GET api/officers/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var officer = await _db.Officers
            .Include(o => o.Department)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (officer is null) return NotFound();
        return Ok(MapToDto(officer));
    }

    // PATCH api/officers/{id}/location
    [HttpPatch("{id:int}/location")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationRequest req)
    {
        var officer = await _db.Officers.FindAsync(id);
        if (officer is null) return NotFound();

        officer.LastLatitude      = req.Latitude;
        officer.LastLongitude     = req.Longitude;
        officer.LastKnownLocation = req.LocationName;
        officer.LastSyncAt        = DateTime.UtcNow;
        officer.UpdatedAt         = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PATCH api/officers/{id}/status
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req)
    {
        var officer = await _db.Officers.FindAsync(id);
        if (officer is null) return NotFound();

        if (!Enum.TryParse<OfficerStatus>(req.Status, true, out var status))
            return BadRequest("Invalid status value.");

        officer.Status    = status;
        officer.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static object MapToDto(Officer o) => new
    {
        o.Id,
        o.BadgeNumber,
        o.FullName,
        o.FirstName,
        o.LastName,
        o.Rank,
        o.Phone,
        o.Email,
        o.DeviceId,
        o.NfcTagId,
        o.Status,
        o.LastKnownLocation,
        o.LastSyncAt,
        o.LastLatitude,
        o.LastLongitude,
        o.IsActive,
        Department = o.Department is null ? null : new
        {
            o.Department.Name,
            o.Department.Zone
        }
    };
}

// ── REQUEST DTOs ──────────────────────────────────────────────
public record UpdateLocationRequest(double Latitude, double Longitude, string LocationName);
public record UpdateStatusRequest(string Status);
