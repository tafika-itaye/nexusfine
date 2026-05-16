using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using NexusFine.Station.Data;

namespace NexusFine.Station.Services;

/// <summary>
/// Pulls reference data from HQ down to the station's local cache.
/// In production this would speak mTLS to https://hq.nexusfine.gov.mw;
/// for the demo we point it at http://localhost:5121 (the HQ API).
///
/// One method per replicated entity. Each writes a `SyncEvent` row on
/// completion so the admin /sync-health surface can report what happened.
/// </summary>
public class HqSyncClient
{
    private readonly HttpClient _http;
    private readonly StationDbContext _db;
    private readonly ILogger<HqSyncClient> _log;

    public HqSyncClient(HttpClient http, StationDbContext db, ILogger<HqSyncClient> log)
    {
        _http = http;
        _db   = db;
        _log  = log;
    }

    public async Task<SyncResult> PullOffenceCodesAsync(CancellationToken ct = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var rows = await _http.GetFromJsonAsync<List<OffenceCodeHqDto>>("api/offencecodes", ct)
                       ?? new List<OffenceCodeHqDto>();

            var byId = await _db.CachedOffenceCodes.ToDictionaryAsync(c => c.Id, ct);
            foreach (var r in rows)
            {
                if (byId.TryGetValue(r.Id, out var existing))
                {
                    existing.Code              = r.Code;
                    existing.Name              = r.Name;
                    existing.DefaultFineAmount = r.DefaultFineAmount;
                    existing.IsActive          = r.IsActive;
                    existing.SyncedAt          = DateTime.UtcNow;
                }
                else
                {
                    _db.CachedOffenceCodes.Add(new CachedOffenceCode
                    {
                        Id = r.Id, Code = r.Code, Name = r.Name,
                        DefaultFineAmount = r.DefaultFineAmount,
                        IsActive = r.IsActive,
                    });
                }
            }

            await _db.SaveChangesAsync(ct);
            await RecordEvent("OffenceCode", "DOWN", rows.Count, "OK", null, ct);
            sw.Stop();
            _log.LogInformation("Pulled {n} offence codes from HQ in {ms}ms", rows.Count, sw.ElapsedMilliseconds);
            return new SyncResult(true, rows.Count, sw.ElapsedMilliseconds, null);
        }
        catch (Exception ex)
        {
            await RecordEvent("OffenceCode", "DOWN", 0, "FAIL", ex.Message, ct);
            _log.LogWarning(ex, "Pull of offence codes from HQ failed");
            return new SyncResult(false, 0, sw.ElapsedMilliseconds, ex.Message);
        }
    }

    private async Task RecordEvent(string type, string dir, int count, string outcome, string? detail, CancellationToken ct)
    {
        _db.SyncEvents.Add(new SyncEvent
        {
            EntityType  = type,
            Direction   = dir,
            RecordCount = count,
            Outcome     = outcome,
            Detail      = detail,
            At          = DateTime.UtcNow,
        });
        var cursor = await _db.SyncCursors
            .FirstOrDefaultAsync(c => c.EntityType == type && c.Direction == dir, ct);
        if (cursor is null)
        {
            cursor = new SyncCursor { EntityType = type, Direction = dir };
            _db.SyncCursors.Add(cursor);
        }
        cursor.LastSyncedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private record OffenceCodeHqDto(int Id, string Code, string Name, string Description, decimal DefaultFineAmount, bool IsActive);
}

public record SyncResult(bool Success, int Records, long Millis, string? Error);
