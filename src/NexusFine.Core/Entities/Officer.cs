namespace NexusFine.Core.Entities;

public class Officer
{
    public int Id { get; set; }
    public string BadgeNumber { get; set; } = string.Empty; // T-0024
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Rank { get; set; } = string.Empty; // Constable, Sergeant, Inspector
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? NfcTagId { get; set; }     // officer ID tag scanned at checkpoints
    public string? DeviceId { get; set; }     // assigned tablet ID e.g. TAB-14
    public string? DeviceImei { get; set; }

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Distributed-arch — see docs/architecture-distributed.md
    public int? StationId { get; set; }
    public Station? Station { get; set; }
    public int? PrimaryPatrolPostId { get; set; }
    public PatrolPost? PrimaryPatrolPost { get; set; }

    public OfficerStatus Status { get; set; } = OfficerStatus.Active;

    /// <summary>
    /// Face-only headshot captured at registration. No biometric matching is
    /// performed — this field is purely for visual identification of the
    /// officer in the admin and (post-Module 4c) tablet UIs. Retention
    /// follows the DPA-2024 schedule recorded in docs/compliance/dpia-malawi.md.
    /// Path is relative to wwwroot, e.g. "img/officers/photos/MPS-0042.jpg".
    /// Null when no photo has been uploaded yet → falls back to silhouette.
    /// </summary>
    public string? PhotoUrl { get; set; }

    // Last known GPS position
    public double? LastLatitude { get; set; }
    public double? LastLongitude { get; set; }
    public string? LastKnownLocation { get; set; } // human-readable zone name
    public DateTime? LastSyncAt { get; set; }

    // Navigation
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
    public string? UserId { get; set; } // ASP.NET Identity link

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public enum OfficerStatus
{
    Active = 0,
    OnBreak = 1,
    OffDuty = 2,
    Suspended = 3,
    Offline = 4
}
