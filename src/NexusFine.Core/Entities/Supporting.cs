namespace NexusFine.Core.Entities;

public class OffenceCode
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;       // e.g. OC-001
    public string Name { get; set; } = string.Empty;       // e.g. Speeding
    public string Description { get; set; } = string.Empty;
    public decimal DefaultFineAmount { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;       // e.g. "Lilongwe Traffic"
    public string Zone { get; set; } = string.Empty;       // district — e.g. "Lilongwe"
    public string Region { get; set; } = string.Empty;     // "Northern" / "Central" / "Southern"
    public string? HeadOfficerBadge { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Officer>  Officers { get; set; } = new List<Officer>();
    public ICollection<Fine>     Fines    { get; set; } = new List<Fine>();
    public ICollection<Station>  Stations { get; set; } = new List<Station>();
}

// ── DISTRIBUTED-ARCH ENTITIES (see docs/architecture-distributed.md) ─────

/// <summary>
/// A police station — sub-unit of a Department.
/// Each Station can run a Station Server (NUC + UPS + 4G failover) that
/// holds a local replica of HQ reference data and is the upstream sync
/// target for officer devices in its area.
/// </summary>
public class Station
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;             // STN-LIL-001
    public string Name { get; set; } = string.Empty;             // "Area 18 Police Station"

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public string Zone { get; set; } = string.Empty;             // denormalised for filter speed
    public string PhysicalAddress { get; set; } = string.Empty;
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public string? ContactPhone { get; set; }
    public string? OfficerInChargeBadge { get; set; }            // station commander

    // Server connectivity (null for stations that don't run a station server yet)
    public string? StationServerEndpoint { get; set; }           // https://stn-lil-001.local:5121
    public string? StationServerPublicKey { get; set; }          // mTLS pin from HQ
    public DateTime? LastSyncAt { get; set; }
    public int      ConsecutiveFailedSyncs { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PatrolPost> PatrolPosts { get; set; } = new List<PatrolPost>();
    public ICollection<Officer>    Officers    { get; set; } = new List<Officer>();
    public ICollection<Device>     Devices     { get; set; } = new List<Device>();
}

/// <summary>
/// A patrol post / checkpoint / beat — sub-unit of a Station.
/// Officers are assigned to one or more PatrolPosts under their station.
/// </summary>
public class PatrolPost
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;             // PP-LIL-018-A
    public string Name { get; set; } = string.Empty;             // "Kamuzu Highway North"

    public int StationId { get; set; }
    public Station Station { get; set; } = null!;

    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// A registered officer-device (Android tablet) paired to a Station.
/// Devices are station property — multiple officers can sign in across
/// shifts (login switch). Heartbeat every 15 min; auto-revokes after 72h
/// of silence (Q7). Full pairing implementation lands in Module 4.
/// </summary>
public class Device
{
    public int Id { get; set; }
    public string Serial { get; set; } = string.Empty;           // tablet serial
    public string? Imei { get; set; }
    public string? Model { get; set; }                           // Samsung Galaxy Tab A8 etc.

    public int StationId { get; set; }
    public Station Station { get; set; } = null!;

    public int? CurrentOfficerId { get; set; }                   // who's signed in right now
    public Officer? CurrentOfficer { get; set; }

    public string AppVersion { get; set; } = string.Empty;
    public DateTime PairedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public DeviceStatus Status { get; set; } = DeviceStatus.Active;
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum DeviceStatus
{
    Active   = 0,
    Stale    = 1,   // no heartbeat for >2h, <72h
    Revoked  = 2,   // auto- or manually revoked
    Retired  = 3
}

public class Appeal
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty; // APP-2026-0091

    public int FineId { get; set; }
    public Fine Fine { get; set; } = null!;

    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string ApplicantPhone { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;

    public AppealStatus Status { get; set; } = AppealStatus.Submitted;
    public string? ReviewerNotes { get; set; }
    public int? ReviewedByOfficerId { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum AppealStatus
{
    Submitted = 0,
    UnderReview = 1,
    Upheld = 2,      // fine cancelled
    Rejected = 3,    // fine stands
    AwaitingInfo = 4
}

public class AuditLog
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;  // Fine, Payment, Officer
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty;      // Created, Updated, Paid, Disputed
    public string? OldValues { get; set; }                  // JSON snapshot
    public string? NewValues { get; set; }                  // JSON snapshot
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
