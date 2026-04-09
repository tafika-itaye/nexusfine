namespace NexusFine.Core.Entities;

public class Fine
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty; // NXF-2026-00481
    public DateTime IssuedAt { get; set; }
    public DateTime DueDate { get; set; }

    // Vehicle
    public string PlateNumber { get; set; } = string.Empty;
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public string? VehicleColour { get; set; }

    // Driver
    public string DriverName { get; set; } = string.Empty;
    public string? DriverNationalId { get; set; }
    public string? DriverLicenceNumber { get; set; }
    public string? DriverPhone { get; set; }

    // Offence
    public int OffenceCodeId { get; set; }
    public OffenceCode OffenceCode { get; set; } = null!;
    public string Location { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
    public string? EvidencePhotoUrl { get; set; }

    // Amount
    public decimal Amount { get; set; }
    public decimal? PenaltyAmount { get; set; } // late payment penalty

    // Status
    public FineStatus Status { get; set; } = FineStatus.Unpaid;

    // Officer
    public int OfficerId { get; set; }
    public Officer Officer { get; set; } = null!;

    // Department / Zone
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Navigation
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Appeal? Appeal { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public enum FineStatus
{
    Unpaid = 0,
    Paid = 1,
    Overdue = 2,
    Disputed = 3,
    Cancelled = 4,
    PartiallyPaid = 5
}
