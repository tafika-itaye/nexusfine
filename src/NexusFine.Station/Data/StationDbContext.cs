using Microsoft.EntityFrameworkCore;

namespace NexusFine.Station.Data;

/// <summary>
/// The Station Server's local store.
///
/// Schema is INTENTIONALLY narrower than the HQ schema — the station only
/// keeps what it needs to operate offline:
///   • A cached, read-only mirror of reference data (OffenceCode, Officer,
///     PatrolPost) for this station's officers.
///   • Its own operational data (counter cash payments, fines issued at the
///     desk, device-pairing rows).
///   • Sync cursors that track what has been pulled from HQ and what is
///     queued to push to HQ.
///
/// The station's primary identity (StationCode) is fixed at deploy time via
/// configuration (Station:Code, Station:HqEndpoint).
/// </summary>
public class StationDbContext : DbContext
{
    public StationDbContext(DbContextOptions<StationDbContext> options) : base(options) { }

    public DbSet<CachedOfficer>      CachedOfficers      => Set<CachedOfficer>();
    public DbSet<CachedOffenceCode>  CachedOffenceCodes  => Set<CachedOffenceCode>();
    public DbSet<CachedPatrolPost>   CachedPatrolPosts   => Set<CachedPatrolPost>();
    public DbSet<PairedDevice>       PairedDevices       => Set<PairedDevice>();
    public DbSet<QueuedRecord>       OutboundQueue       => Set<QueuedRecord>();
    public DbSet<SyncCursor>         SyncCursors         => Set<SyncCursor>();
    public DbSet<SyncEvent>          SyncEvents          => Set<SyncEvent>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<CachedOfficer>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasIndex(o => o.BadgeNumber).IsUnique();
            e.Property(o => o.BadgeNumber).HasMaxLength(15).IsRequired();
            e.Property(o => o.FullName).HasMaxLength(120).IsRequired();
            e.Property(o => o.Rank).HasMaxLength(40).IsRequired();
        });

        mb.Entity<CachedOffenceCode>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasIndex(o => o.Code).IsUnique();
            e.Property(o => o.Code).HasMaxLength(10).IsRequired();
            e.Property(o => o.Name).HasMaxLength(120).IsRequired();
        });

        mb.Entity<CachedPatrolPost>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Code).IsUnique();
            e.Property(p => p.Code).HasMaxLength(20).IsRequired();
        });

        mb.Entity<PairedDevice>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.Serial).IsUnique();
            e.Property(d => d.Serial).HasMaxLength(60).IsRequired();
            e.Property(d => d.PairingTokenHash).HasMaxLength(128).IsRequired();
        });

        mb.Entity<QueuedRecord>(e =>
        {
            e.HasKey(q => q.Id);
            e.HasIndex(q => new { q.EntityType, q.SyncedAt });
            e.Property(q => q.EntityType).HasMaxLength(40).IsRequired();
            e.Property(q => q.ClientUuid).HasMaxLength(40).IsRequired();
            e.HasIndex(q => q.ClientUuid).IsUnique();   // idempotency anchor
        });

        mb.Entity<SyncCursor>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => new { c.EntityType, c.Direction }).IsUnique();
            e.Property(c => c.EntityType).HasMaxLength(40).IsRequired();
            e.Property(c => c.Direction).HasMaxLength(8).IsRequired();   // "DOWN" / "UP"
        });

        mb.Entity<SyncEvent>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.At);
            e.Property(s => s.EntityType).HasMaxLength(40).IsRequired();
            e.Property(s => s.Direction).HasMaxLength(8).IsRequired();
            e.Property(s => s.Outcome).HasMaxLength(20).IsRequired();
        });
    }
}

public class CachedOfficer
{
    public int Id { get; set; }
    public string BadgeNumber { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Rank { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class CachedOffenceCode
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal DefaultFineAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class CachedPatrolPost
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public class PairedDevice
{
    public int Id { get; set; }
    public string Serial { get; set; } = "";
    public string PairingTokenHash { get; set; } = "";
    public DateTime PairedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastHeartbeatAt { get; set; }
    public bool Revoked { get; set; }
}

/// <summary>
/// A record waiting to be pushed up to HQ. Stores the serialised payload
/// so the originating service code doesn't need to be available at sync
/// time; the sync engine just forwards the JSON.
/// </summary>
public class QueuedRecord
{
    public long Id { get; set; }
    public string ClientUuid { get; set; } = "";        // idempotency key
    public string EntityType { get; set; } = "";        // "Fine" / "Payment" / "OfficerStatus"
    public string JsonPayload { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SyncedAt { get; set; }
    public int Attempts { get; set; }
    public string? LastError { get; set; }
}

public class SyncCursor
{
    public int Id { get; set; }
    public string EntityType { get; set; } = "";
    public string Direction { get; set; } = "DOWN";
    public DateTime LastSyncedAt { get; set; }
    public string? VersionTag { get; set; }
}

public class SyncEvent
{
    public long Id { get; set; }
    public string EntityType { get; set; } = "";
    public string Direction { get; set; } = "";
    public int RecordCount { get; set; }
    public string Outcome { get; set; } = "";           // "OK" / "FAIL" / "PARTIAL"
    public string? Detail { get; set; }
    public DateTime At { get; set; } = DateTime.UtcNow;
}
