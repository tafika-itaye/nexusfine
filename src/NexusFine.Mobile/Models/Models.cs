namespace NexusFine.Mobile.Models;

// ── FINE ──────────────────────────────────────────────────────
public class Fine
{
    public int      Id               { get; set; }
    public string   ReferenceNumber  { get; set; } = string.Empty;
    public string   PlateNumber      { get; set; } = string.Empty;
    public string   DriverName       { get; set; } = string.Empty;
    public string?  DriverNationalId { get; set; }
    public string?  DriverPhone      { get; set; }
    public string   Location         { get; set; } = string.Empty;
    public decimal  Amount           { get; set; }
    public string   Status           { get; set; } = string.Empty;
    public DateTime IssuedAt         { get; set; }
    public DateTime DueDate          { get; set; }
    public OffenceCodeDto? OffenceCode { get; set; }

    public string StatusColour => Status switch
    {
        "Paid"    => "#22a060",
        "Overdue" => "#d97706",
        "Disputed"=> "#c8960c",
        _         => "#e74c3c"
    };

    public string FormattedAmount => $"MK {Amount:N0}";
}

// ── OFFENCE CODE ──────────────────────────────────────────────
public class OffenceCodeDto
{
    public int     Id                { get; set; }
    public string  Code              { get; set; } = string.Empty;
    public string  Name              { get; set; } = string.Empty;
    public string  Description       { get; set; } = string.Empty;
    public decimal DefaultFineAmount { get; set; }
}

// ── OFFICER ───────────────────────────────────────────────────
public class Officer
{
    public int     Id              { get; set; }
    public string  BadgeNumber     { get; set; } = string.Empty;
    public string  FullName        { get; set; } = string.Empty;
    public string  Rank            { get; set; } = string.Empty;
    public string  Status          { get; set; } = string.Empty;
    public string? Zone            { get; set; }
    public string? LastKnownLocation { get; set; }
    public int     FinesIssued     { get; set; }
    public int     FinesCollected  { get; set; }
    public double  CollectionRate  { get; set; }
}

// ── CREATE FINE REQUEST ───────────────────────────────────────
public class CreateFineRequest
{
    public string  PlateNumber         { get; set; } = string.Empty;
    public string  DriverName          { get; set; } = string.Empty;
    public int     OffenceCodeId        { get; set; }
    public int     OfficerId            { get; set; }
    public string  Location             { get; set; } = string.Empty;
    public string? VehicleMake          { get; set; }
    public string? VehicleModel         { get; set; }
    public string? VehicleColour        { get; set; }
    public string? DriverNationalId     { get; set; }
    public string? DriverLicenceNumber  { get; set; }
    public string? DriverPhone          { get; set; }
    public double? Latitude             { get; set; }
    public double? Longitude            { get; set; }
    public string? Notes                { get; set; }
}

// ── KPI ───────────────────────────────────────────────────────
public class KpiDto
{
    public int     IssuedToday     { get; set; }
    public decimal RevenueMonth    { get; set; }
    public decimal CollectionRate  { get; set; }
    public int     OfficersOnDuty  { get; set; }
    public int     OfficersTotal   { get; set; }
}

// ── PAGED RESULT ──────────────────────────────────────────────
public class PagedResult<T>
{
    public int       Total    { get; set; }
    public int       Page     { get; set; }
    public int       PageSize { get; set; }
    public List<T>   Data     { get; set; } = new();
}
