using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supervisor")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    public DevicesController(AppDbContext db) => _db = db;

    // GET api/devices?stationId=&status=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? stationId, [FromQuery] string? status)
    {
        IQueryable<Device> q = _db.Devices
            .Include(d => d.Station)
            .Include(d => d.CurrentOfficer);

        if (stationId.HasValue) q = q.Where(d => d.StationId == stationId.Value);
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<DeviceStatus>(status, true, out var st))
            q = q.Where(d => d.Status == st);

        var list = await q.OrderByDescending(d => d.LastSeenAt)
            .Select(d => new
            {
                d.Id, d.Serial, d.Imei, d.Model,
                d.StationId,
                StationCode = d.Station.Code,
                StationName = d.Station.Name,
                d.AppVersion,
                d.PairedAt, d.LastSeenAt, d.LastSyncAt,
                d.Status, d.RevokedAt, d.RevokedReason,
                CurrentOfficerBadge = d.CurrentOfficer == null ? null : d.CurrentOfficer.BadgeNumber,
                CurrentOfficerName  = d.CurrentOfficer == null ? null : d.CurrentOfficer.FirstName + " " + d.CurrentOfficer.LastName
            })
            .ToListAsync();

        return Ok(list);
    }

    // POST api/devices/{id}/revoke
    [HttpPost("{id:int}/revoke")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Revoke(int id, [FromBody] DeviceRevokeReq req)
    {
        var d = await _db.Devices.FindAsync(id);
        if (d is null) return NotFound();

        d.Status        = DeviceStatus.Revoked;
        d.RevokedAt     = DateTime.UtcNow;
        d.RevokedReason = string.IsNullOrWhiteSpace(req.Reason) ? "Manual revoke" : req.Reason.Trim();
        d.UpdatedAt     = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { d.Id, d.Status });
    }
}

public record DeviceRevokeReq(string? Reason);
