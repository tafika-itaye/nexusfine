using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusFine.Infrastructure.Data;

namespace NexusFine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // any authenticated role can read offence codes
public class OffenceCodesController : ControllerBase
{
    private readonly AppDbContext _db;
    public OffenceCodesController(AppDbContext db) => _db = db;

    // GET api/offencecodes
    [HttpGet]
    [AllowAnonymous] // public — needed for citizen portal lookups
    public async Task<IActionResult> GetAll()
    {
        var codes = await _db.OffenceCodes
            .Where(o => o.IsActive)
            .OrderBy(o => o.Code)
            .Select(o => new
            {
                o.Id,
                o.Code,
                o.Name,
                o.Description,
                o.DefaultFineAmount
            })
            .ToListAsync();

        return Ok(codes);
    }
}
