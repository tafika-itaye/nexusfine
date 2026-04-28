using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // any authenticated role can read offence codes
public class OffenceCodesController : ControllerBase
{
    private readonly AppDbContext _db;
    public OffenceCodesController(AppDbContext db) => _db = db;

    // GET api/offencecodes?includeInactive=true
    [HttpGet]
    [AllowAnonymous] // public — needed for citizen portal lookups
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        IQueryable<OffenceCode> q = _db.OffenceCodes;
        if (!includeInactive) q = q.Where(o => o.IsActive);

        var codes = await q.OrderBy(o => o.Code)
            .Select(o => new
            {
                o.Id, o.Code, o.Name, o.Description, o.DefaultFineAmount, o.IsActive
            })
            .ToListAsync();

        return Ok(codes);
    }

    // POST api/offencecodes
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] OffenceCodeUpsert req)
    {
        if (string.IsNullOrWhiteSpace(req.Code) || string.IsNullOrWhiteSpace(req.Name))
            return BadRequest(new { message = "Code and Name are required." });

        if (req.DefaultFineAmount < 0)
            return BadRequest(new { message = "DefaultFineAmount must be non-negative." });

        if (await _db.OffenceCodes.AnyAsync(o => o.Code == req.Code))
            return Conflict(new { message = $"Offence code '{req.Code}' is already in use." });

        var code = new OffenceCode
        {
            Code              = req.Code.Trim(),
            Name              = req.Name.Trim(),
            Description       = req.Description?.Trim() ?? "",
            DefaultFineAmount = req.DefaultFineAmount,
            IsActive          = req.IsActive
        };
        _db.OffenceCodes.Add(code);
        await _db.SaveChangesAsync();
        return Created($"api/offencecodes/{code.Id}", new { code.Id, code.Code });
    }

    // PUT api/offencecodes/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] OffenceCodeUpsert req)
    {
        var c = await _db.OffenceCodes.FindAsync(id);
        if (c is null) return NotFound();

        if (req.DefaultFineAmount < 0)
            return BadRequest(new { message = "DefaultFineAmount must be non-negative." });

        if (!string.IsNullOrWhiteSpace(req.Code) && req.Code != c.Code &&
            await _db.OffenceCodes.AnyAsync(o => o.Code == req.Code && o.Id != id))
            return Conflict(new { message = $"Offence code '{req.Code}' is already in use." });

        c.Code              = req.Code?.Trim() ?? c.Code;
        c.Name              = req.Name?.Trim() ?? c.Name;
        c.Description       = req.Description?.Trim() ?? c.Description;
        c.DefaultFineAmount = req.DefaultFineAmount;
        c.IsActive          = req.IsActive;
        await _db.SaveChangesAsync();
        return Ok(new { c.Id, c.Code });
    }

    // DELETE api/offencecodes/{id}  (soft — toggles IsActive=false)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var c = await _db.OffenceCodes.FindAsync(id);
        if (c is null) return NotFound();

        if (await _db.Fines.AnyAsync(f => f.OffenceCodeId == id))
        {
            // Don't break referential integrity — just deactivate.
            c.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { soft = true, c.Id, c.IsActive });
        }

        // Truly unused — hard delete is fine.
        _db.OffenceCodes.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record OffenceCodeUpsert(
    string  Code,
    string  Name,
    string? Description,
    decimal DefaultFineAmount,
    bool    IsActive
);
