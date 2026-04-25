using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinesController : ControllerBase
{
    private readonly AppDbContext _db;

    public FinesController(AppDbContext db) => _db = db;

    // GET api/fines/lookup?type=plate&value=MWK1234A
    // Public — citizen portal uses this to look up their own fine.
    [AllowAnonymous]
    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string type, [FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return BadRequest("Search value is required.");

        Fine? fine = type.ToLower() switch
        {
            "plate" => await _db.Fines
                .Include(f => f.OffenceCode)
                .Include(f => f.Officer)
                .Include(f => f.Payments)
                .Include(f => f.Appeal)
                .FirstOrDefaultAsync(f => f.PlateNumber == value.ToUpper().Trim()),

            "ref" => await _db.Fines
                .Include(f => f.OffenceCode)
                .Include(f => f.Officer)
                .Include(f => f.Payments)
                .Include(f => f.Appeal)
                .FirstOrDefaultAsync(f => f.ReferenceNumber == value.ToUpper().Trim()),

            "id" => await _db.Fines
                .Include(f => f.OffenceCode)
                .Include(f => f.Officer)
                .Include(f => f.Payments)
                .Include(f => f.Appeal)
                .FirstOrDefaultAsync(f => f.DriverNationalId == value.Trim()),

            _ => null
        };

        if (fine is null)
            return NotFound(new { message = "No fine found matching the provided details." });

        return Ok(MapToDto(fine));
    }

    // GET api/fines/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var fine = await _db.Fines
            .Include(f => f.OffenceCode)
            .Include(f => f.Officer)
            .Include(f => f.Department)
            .Include(f => f.Payments)
            .Include(f => f.Appeal)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fine is null) return NotFound();
        return Ok(MapToDto(fine));
    }

    // GET api/fines?departmentId=1&from=2026-04-01&to=2026-04-08&status=Unpaid
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? departmentId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _db.Fines
            .Include(f => f.OffenceCode)
            .Include(f => f.Officer)
            .Include(f => f.Department)
            .AsQueryable();

        if (departmentId.HasValue)
            query = query.Where(f => f.DepartmentId == departmentId.Value);

        if (from.HasValue)
            query = query.Where(f => f.IssuedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(f => f.IssuedAt <= to.Value);

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<FineStatus>(status, true, out var fineStatus))
            query = query.Where(f => f.Status == fineStatus);

        var total = await query.CountAsync();
        var fines = await query
            .OrderByDescending(f => f.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            data = fines.Select(MapToDto)
        });
    }

    // POST api/fines  (officer issues a fine)
    [Authorize(Roles = "Officer,Supervisor,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFineRequest req)
    {
        var offenceCode = await _db.OffenceCodes.FindAsync(req.OffenceCodeId);
        if (offenceCode is null)
            return BadRequest("Invalid offence code.");

        var officer = await _db.Officers.FindAsync(req.OfficerId);
        if (officer is null)
            return BadRequest("Invalid officer.");

        var fine = new Fine
        {
            ReferenceNumber     = GenerateReference(),
            IssuedAt            = DateTime.UtcNow,
            DueDate             = DateTime.UtcNow.AddDays(30),
            PlateNumber         = req.PlateNumber.ToUpper().Trim(),
            VehicleMake         = req.VehicleMake,
            VehicleModel        = req.VehicleModel,
            VehicleColour       = req.VehicleColour,
            DriverName          = req.DriverName,
            DriverNationalId    = req.DriverNationalId,
            DriverLicenceNumber = req.DriverLicenceNumber,
            DriverPhone         = req.DriverPhone,
            OffenceCodeId       = req.OffenceCodeId,
            Location            = req.Location,
            Latitude            = req.Latitude,
            Longitude           = req.Longitude,
            Notes               = req.Notes,
            Amount              = offenceCode.DefaultFineAmount,
            OfficerId           = req.OfficerId,
            DepartmentId        = officer.DepartmentId,
            Status              = FineStatus.Unpaid
        };

        _db.Fines.Add(fine);
        await _db.SaveChangesAsync();

        // TODO: trigger SMS notification to DriverPhone

        return CreatedAtAction(nameof(GetById), new { id = fine.Id }, MapToDto(fine));
    }

    // ── HELPERS ───────────────────────────────────────────────
    private string GenerateReference()
    {
        var year  = DateTime.UtcNow.Year;
        var count = _db.Fines.Count() + 1;
        return $"NXF-{year}-{count:D5}";
    }

    private static object MapToDto(Fine f) => new
    {
        f.Id,
        f.ReferenceNumber,
        f.IssuedAt,
        f.DueDate,
        f.PlateNumber,
        f.VehicleMake,
        f.VehicleModel,
        f.VehicleColour,
        f.DriverName,
        f.DriverNationalId,
        f.DriverLicenceNumber,
        f.DriverPhone,
        f.Location,
        f.Latitude,
        f.Longitude,
        f.Amount,
        f.PenaltyAmount,
        f.Status,
        f.Notes,
        f.EvidencePhotoUrl,
        f.CreatedAt,
        OffenceCode = f.OffenceCode is null ? null : new
        {
            f.OffenceCode.Code,
            f.OffenceCode.Name,
            f.OffenceCode.Description
        },
        Officer = f.Officer is null ? null : new
        {
            f.Officer.BadgeNumber,
            f.Officer.FullName,
            f.Officer.Rank
        },
        Payments = f.Payments?.Select(p => new
        {
            p.ReceiptNumber,
            p.Amount,
            p.Channel,
            p.Status,
            p.CompletedAt
        }),
        Appeal = f.Appeal is null ? null : new
        {
            f.Appeal.ReferenceNumber,
            f.Appeal.Status,
            f.Appeal.SubmittedAt
        }
    };
}

// ── REQUEST DTOs ──────────────────────────────────────────────
public record CreateFineRequest(
    string PlateNumber,
    string DriverName,
    int    OffenceCodeId,
    int    OfficerId,
    string Location,
    string? VehicleMake,
    string? VehicleModel,
    string? VehicleColour,
    string? DriverNationalId,
    string? DriverLicenceNumber,
    string? DriverPhone,
    double? Latitude,
    double? Longitude,
    string? Notes
);
