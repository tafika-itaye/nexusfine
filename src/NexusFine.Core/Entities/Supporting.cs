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
    public string Name { get; set; } = string.Empty;       // e.g. Lilongwe Traffic
    public string Zone { get; set; } = string.Empty;       // e.g. Lilongwe
    public string? HeadOfficerBadge { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Officer> Officers { get; set; } = new List<Officer>();
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
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
