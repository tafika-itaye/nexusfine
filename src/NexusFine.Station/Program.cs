using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NexusFine.Station.Data;
using NexusFine.Station.Services;

var builder = WebApplication.CreateBuilder(args);

// ── CONFIGURATION ────────────────────────────────────────────
// Station identity is set at deploy time. For dev / demo we default to STN-LIL-001.
var stationCode    = builder.Configuration["Station:Code"]        ?? "STN-LIL-001";
var stationName    = builder.Configuration["Station:Name"]        ?? "Area 18 Police Station (Pilot)";
var hqEndpoint     = builder.Configuration["Station:HqEndpoint"]  ?? "http://localhost:5121";
var dbPath         = builder.Configuration["Station:DbPath"]      ?? Path.Combine(AppContext.BaseDirectory, "station.db");

// ── SERVICES ─────────────────────────────────────────────────
builder.Services.AddDbContext<StationDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

builder.Services
    .AddHttpClient<HqSyncClient>(c =>
    {
        c.BaseAddress = new Uri(hqEndpoint);
        c.DefaultRequestHeaders.Add("X-Station-Code", stationCode);
        c.Timeout = TimeSpan.FromSeconds(15);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── APP ──────────────────────────────────────────────────────
var app = builder.Build();

// Ensure schema exists (SQLite — small footprint, no migration ceremony needed)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StationDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ── ENDPOINTS ───────────────────────────────────────────────

// Health — for the runbook pre-flight and Azure/k8s liveness.
app.MapGet("/api/health", () => Results.Ok(new
{
    status      = "ok",
    service     = "NexusFine.Station",
    stationCode,
    stationName,
    hqEndpoint,
    timestampUtc = DateTime.UtcNow,
}));

// Station identity card.
app.MapGet("/api/station/info", async (StationDbContext db) =>
{
    var lastSync = await db.SyncCursors.OrderByDescending(c => c.LastSyncedAt).FirstOrDefaultAsync();
    var cached   = new
    {
        offenceCodes = await db.CachedOffenceCodes.CountAsync(),
        officers     = await db.CachedOfficers.CountAsync(),
        patrolPosts  = await db.CachedPatrolPosts.CountAsync(),
        devices      = await db.PairedDevices.CountAsync(d => !d.Revoked),
        outboundQ    = await db.OutboundQueue.CountAsync(q => q.SyncedAt == null),
    };
    return Results.Ok(new
    {
        stationCode,
        stationName,
        hqEndpoint,
        lastSync = lastSync is null ? (object)"never" : new
        {
            lastSync.EntityType,
            lastSync.Direction,
            lastSync.LastSyncedAt,
        },
        cached,
    });
});

// ── SYNC ───
// Trigger a pull from HQ. Manual button for the demo runbook
// ("show the station catching up after the network comes back").
app.MapPost("/api/sync/pull", async (HqSyncClient client) =>
{
    var oc = await client.PullOffenceCodesAsync();
    return Results.Ok(new
    {
        offenceCodes = oc,
        // Stubbed in this build — same client method shape lands for
        // officers + patrol-posts in Module 4b.
        officers     = new { skipped = true, reason = "Pull endpoint lands in Module 4b" },
        patrolPosts  = new { skipped = true, reason = "Pull endpoint lands in Module 4b" },
    });
});

// Get the last N sync events (admin/health surface).
app.MapGet("/api/sync/events", async (StationDbContext db, int limit) =>
{
    if (limit <= 0) limit = 20;
    var rows = await db.SyncEvents.OrderByDescending(e => e.At).Take(limit).ToListAsync();
    return Results.Ok(rows);
});

// ── DEVICE PAIRING ───
// One-time token mint. In production this lives behind supervisor auth.
app.MapPost("/api/devices/pair", async (PairRequest req, StationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.Serial))
        return Results.BadRequest(new { message = "Serial is required." });

    if (await db.PairedDevices.AnyAsync(d => d.Serial == req.Serial && !d.Revoked))
        return Results.Conflict(new { message = "Device already paired. Revoke first." });

    // 10-minute one-time token. Stored hashed.
    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24)).Replace('+','-').Replace('/','_').TrimEnd('=');
    var hash  = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    db.PairedDevices.Add(new PairedDevice
    {
        Serial = req.Serial,
        PairingTokenHash = hash,
        PairedAt = DateTime.UtcNow,
    });
    await db.SaveChangesAsync();

    return Results.Created($"/api/devices/{req.Serial}", new
    {
        serial    = req.Serial,
        token     = token,                                // shown ONCE
        expiresAt = DateTime.UtcNow.AddMinutes(10),
        stationCode,
    });
});

// Device heartbeat — lightweight, called every 15 min from a tablet.
app.MapPost("/api/devices/{serial}/heartbeat", async (string serial, StationDbContext db) =>
{
    var dev = await db.PairedDevices.FirstOrDefaultAsync(d => d.Serial == serial && !d.Revoked);
    if (dev is null) return Results.NotFound(new { message = "Device not paired or revoked." });
    dev.LastHeartbeatAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(new { ack = true, dev.LastHeartbeatAt });
});

// Operational push (stubbed — Module 4b implements full batch validation).
app.MapPost("/api/ingest", async (IngestBatch batch, StationDbContext db) =>
{
    if (batch.Records is null || batch.Records.Count == 0)
        return Results.BadRequest(new { message = "Empty batch." });

    int accepted = 0, duplicate = 0;
    foreach (var r in batch.Records)
    {
        if (await db.OutboundQueue.AnyAsync(q => q.ClientUuid == r.ClientUuid))
        {
            duplicate++;
            continue;
        }
        db.OutboundQueue.Add(new QueuedRecord
        {
            ClientUuid   = r.ClientUuid,
            EntityType   = r.EntityType,
            JsonPayload  = r.JsonPayload,
            CreatedAt    = DateTime.UtcNow,
        });
        accepted++;
    }
    await db.SaveChangesAsync();
    return Results.Ok(new { accepted, duplicate, queued = await db.OutboundQueue.CountAsync(q => q.SyncedAt == null) });
});

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

// ── DTOs ──
public record PairRequest(string Serial);
public record IngestBatch(List<IngestRecord> Records);
public record IngestRecord(string ClientUuid, string EntityType, string JsonPayload);
