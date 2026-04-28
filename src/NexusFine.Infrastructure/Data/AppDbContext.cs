using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;

namespace NexusFine.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── TABLES ──
    public DbSet<Fine> Fines => Set<Fine>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Officer> Officers => Set<Officer>();
    public DbSet<OffenceCode> OffenceCodes => Set<OffenceCode>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Appeal> Appeals => Set<Appeal>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();

    // Distributed-arch
    public DbSet<Station>     Stations     => Set<Station>();
    public DbSet<PatrolPost>  PatrolPosts  => Set<PatrolPost>();
    public DbSet<Device>      Devices      => Set<Device>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ── FINE ──
        mb.Entity<Fine>(e =>
        {
            e.HasKey(f => f.Id);
            e.HasIndex(f => f.ReferenceNumber).IsUnique();
            e.HasIndex(f => f.PlateNumber);
            e.HasIndex(f => f.DriverNationalId);
            e.HasIndex(f => f.Status);
            e.HasIndex(f => f.IssuedAt);

            e.Property(f => f.ReferenceNumber).HasMaxLength(20).IsRequired();
            e.Property(f => f.PlateNumber).HasMaxLength(15).IsRequired();
            e.Property(f => f.DriverName).HasMaxLength(120).IsRequired();
            e.Property(f => f.DriverNationalId).HasMaxLength(30);
            e.Property(f => f.DriverLicenceNumber).HasMaxLength(30);
            e.Property(f => f.DriverPhone).HasMaxLength(20);
            e.Property(f => f.Location).HasMaxLength(255).IsRequired();
            e.Property(f => f.Amount).HasColumnType("decimal(18,2)");
            e.Property(f => f.PenaltyAmount).HasColumnType("decimal(18,2)");
            e.Property(f => f.Status).HasConversion<string>();

            e.HasOne(f => f.Officer)
             .WithMany(o => o.Fines)
             .HasForeignKey(f => f.OfficerId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(f => f.OffenceCode)
             .WithMany(oc => oc.Fines)
             .HasForeignKey(f => f.OffenceCodeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(f => f.Department)
             .WithMany(d => d.Fines)
             .HasForeignKey(f => f.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(f => f.Payments)
             .WithOne(p => p.Fine)
             .HasForeignKey(p => p.FineId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(f => f.Appeal)
             .WithOne(a => a.Fine)
             .HasForeignKey<Appeal>(a => a.FineId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── PAYMENT ──
        mb.Entity<Payment>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.ReceiptNumber).IsUnique();
            e.HasIndex(p => p.TransactionReference);
            e.HasIndex(p => p.Status);

            e.Property(p => p.ReceiptNumber).HasMaxLength(25).IsRequired();
            e.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            e.Property(p => p.Channel).HasConversion<string>();
            e.Property(p => p.Status).HasConversion<string>();
            e.Property(p => p.PhoneNumber).HasMaxLength(20);
            e.Property(p => p.TransactionReference).HasMaxLength(100);
        });

        // ── OFFICER ──
        mb.Entity<Officer>(e =>
        {
            e.HasKey(o => o.Id);
            e.HasIndex(o => o.BadgeNumber).IsUnique();
            e.HasIndex(o => o.NfcTagId);
            e.HasIndex(o => o.DeviceId);

            e.Property(o => o.BadgeNumber).HasMaxLength(15).IsRequired();
            e.Property(o => o.FirstName).HasMaxLength(60).IsRequired();
            e.Property(o => o.LastName).HasMaxLength(60).IsRequired();
            e.Property(o => o.Rank).HasMaxLength(40).IsRequired();
            e.Property(o => o.Phone).HasMaxLength(20);
            e.Property(o => o.NfcTagId).HasMaxLength(50);
            e.Property(o => o.DeviceId).HasMaxLength(20);
            e.Property(o => o.LastKnownLocation).HasMaxLength(120);
            e.Property(o => o.Status).HasConversion<string>();

            e.Ignore(o => o.FullName); // computed property

            e.HasOne(o => o.Department)
             .WithMany(d => d.Officers)
             .HasForeignKey(o => o.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.Station)
             .WithMany(s => s.Officers)
             .HasForeignKey(o => o.StationId)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(o => o.PrimaryPatrolPost)
             .WithMany()
             .HasForeignKey(o => o.PrimaryPatrolPostId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── STATION ──
        mb.Entity<Station>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.Code).IsUnique();
            e.HasIndex(s => s.Zone);

            e.Property(s => s.Code).HasMaxLength(20).IsRequired();
            e.Property(s => s.Name).HasMaxLength(120).IsRequired();
            e.Property(s => s.Zone).HasMaxLength(60).IsRequired();
            e.Property(s => s.PhysicalAddress).HasMaxLength(255);
            e.Property(s => s.ContactPhone).HasMaxLength(20);
            e.Property(s => s.OfficerInChargeBadge).HasMaxLength(15);
            e.Property(s => s.StationServerEndpoint).HasMaxLength(255);
            e.Property(s => s.StationServerPublicKey).HasMaxLength(2048);
            e.Property(s => s.Lat).HasColumnType("decimal(9,6)");
            e.Property(s => s.Lng).HasColumnType("decimal(9,6)");

            e.HasOne(s => s.Department)
             .WithMany(d => d.Stations)
             .HasForeignKey(s => s.DepartmentId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── PATROL POST ──
        mb.Entity<PatrolPost>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Code).IsUnique();
            e.HasIndex(p => p.StationId);

            e.Property(p => p.Code).HasMaxLength(20).IsRequired();
            e.Property(p => p.Name).HasMaxLength(120).IsRequired();
            e.Property(p => p.Notes).HasMaxLength(500);
            e.Property(p => p.Lat).HasColumnType("decimal(9,6)");
            e.Property(p => p.Lng).HasColumnType("decimal(9,6)");

            // Restrict (was Cascade) to avoid the multi-cascade-path conflict
            // with Officer.StationId / Officer.PrimaryPatrolPostId on Station deletion.
            // We soft-delete stations anyway, so the cascade was never going to fire
            // operationally; this just satisfies SQL Server's static analysis.
            e.HasOne(p => p.Station)
             .WithMany(s => s.PatrolPosts)
             .HasForeignKey(p => p.StationId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── DEVICE ──
        mb.Entity<Device>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.Serial).IsUnique();
            e.HasIndex(d => d.StationId);
            e.HasIndex(d => d.Status);

            e.Property(d => d.Serial).HasMaxLength(60).IsRequired();
            e.Property(d => d.Imei).HasMaxLength(20);
            e.Property(d => d.Model).HasMaxLength(80);
            e.Property(d => d.AppVersion).HasMaxLength(20);
            e.Property(d => d.RevokedReason).HasMaxLength(255);
            e.Property(d => d.Status).HasConversion<string>();

            e.HasOne(d => d.Station)
             .WithMany(s => s.Devices)
             .HasForeignKey(d => d.StationId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(d => d.CurrentOfficer)
             .WithMany()
             .HasForeignKey(d => d.CurrentOfficerId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── OFFENCE CODE ──
        mb.Entity<OffenceCode>(e =>
        {
            e.HasKey(oc => oc.Id);
            e.HasIndex(oc => oc.Code).IsUnique();
            e.Property(oc => oc.Code).HasMaxLength(10).IsRequired();
            e.Property(oc => oc.Name).HasMaxLength(100).IsRequired();
            e.Property(oc => oc.Description).HasMaxLength(500);
            e.Property(oc => oc.DefaultFineAmount).HasColumnType("decimal(18,2)");
        });

        // ── DEPARTMENT ──
        mb.Entity<Department>(e =>
        {
            e.HasKey(d => d.Id);
            e.HasIndex(d => d.Region);
            e.Property(d => d.Name).HasMaxLength(100).IsRequired();
            e.Property(d => d.Zone).HasMaxLength(60).IsRequired();
            e.Property(d => d.Region).HasMaxLength(30).IsRequired();
            e.Property(d => d.HeadOfficerBadge).HasMaxLength(15);
        });

        // ── APPEAL ──
        mb.Entity<Appeal>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasIndex(a => a.ReferenceNumber).IsUnique();
            e.Property(a => a.ReferenceNumber).HasMaxLength(20).IsRequired();
            e.Property(a => a.Reason).HasMaxLength(200).IsRequired();
            e.Property(a => a.ApplicantPhone).HasMaxLength(20).IsRequired();
            e.Property(a => a.ApplicantName).HasMaxLength(120).IsRequired();
            e.Property(a => a.Status).HasConversion<string>();
        });

        // ── AUDIT LOG ──
        mb.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasIndex(a => a.EntityType);
            e.HasIndex(a => a.Timestamp);
            e.Property(a => a.EntityType).HasMaxLength(50).IsRequired();
            e.Property(a => a.Action).HasMaxLength(50).IsRequired();
            e.Property(a => a.UserId).HasMaxLength(100);
            e.Property(a => a.IpAddress).HasMaxLength(45);
        });

        // ── APP USER ──
        mb.Entity<AppUser>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.UserName).IsUnique();
            e.HasIndex(u => u.Email);
            e.Property(u => u.UserName).HasMaxLength(50).IsRequired();
            e.Property(u => u.Email).HasMaxLength(120).IsRequired();
            e.Property(u => u.FullName).HasMaxLength(120).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            e.Property(u => u.PasswordSalt).HasMaxLength(255).IsRequired();
            e.Property(u => u.Roles).HasMaxLength(120).IsRequired();

            e.HasOne(u => u.Officer)
             .WithMany()
             .HasForeignKey(u => u.OfficerId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // ── SEED DATA ──
        // 28 Departments — one per Malawi district, grouped into the
        // three administrative regions (Northern, Central, Southern).
        // Existing IDs 1-4 preserved (they back the demo fines/payments seed).
        mb.Entity<Department>().HasData(
            // Central
            new Department { Id = 1,  Name = "Lilongwe Traffic",   Zone = "Lilongwe",   Region = "Central",  IsActive = true },
            // Southern
            new Department { Id = 2,  Name = "Blantyre Traffic",   Zone = "Blantyre",   Region = "Southern", IsActive = true },
            // Northern (Mzuzu city sits in Mzimba district)
            new Department { Id = 3,  Name = "Mzuzu Traffic",      Zone = "Mzimba",     Region = "Northern", IsActive = true },
            // Southern
            new Department { Id = 4,  Name = "Zomba Traffic",      Zone = "Zomba",      Region = "Southern", IsActive = true },

            // Northern region (5 more)
            new Department { Id = 5,  Name = "Chitipa Traffic",    Zone = "Chitipa",    Region = "Northern", IsActive = true },
            new Department { Id = 6,  Name = "Karonga Traffic",    Zone = "Karonga",    Region = "Northern", IsActive = true },
            new Department { Id = 7,  Name = "Likoma Traffic",     Zone = "Likoma",     Region = "Northern", IsActive = true },
            new Department { Id = 8,  Name = "Nkhata Bay Traffic", Zone = "Nkhata Bay", Region = "Northern", IsActive = true },
            new Department { Id = 9,  Name = "Rumphi Traffic",     Zone = "Rumphi",     Region = "Northern", IsActive = true },

            // Central region (8 more)
            new Department { Id = 10, Name = "Dedza Traffic",      Zone = "Dedza",      Region = "Central",  IsActive = true },
            new Department { Id = 11, Name = "Dowa Traffic",       Zone = "Dowa",       Region = "Central",  IsActive = true },
            new Department { Id = 12, Name = "Kasungu Traffic",    Zone = "Kasungu",    Region = "Central",  IsActive = true },
            new Department { Id = 13, Name = "Mchinji Traffic",    Zone = "Mchinji",    Region = "Central",  IsActive = true },
            new Department { Id = 14, Name = "Nkhotakota Traffic", Zone = "Nkhotakota", Region = "Central",  IsActive = true },
            new Department { Id = 15, Name = "Ntcheu Traffic",     Zone = "Ntcheu",     Region = "Central",  IsActive = true },
            new Department { Id = 16, Name = "Ntchisi Traffic",    Zone = "Ntchisi",    Region = "Central",  IsActive = true },
            new Department { Id = 17, Name = "Salima Traffic",     Zone = "Salima",     Region = "Central",  IsActive = true },

            // Southern region (11 more)
            new Department { Id = 18, Name = "Balaka Traffic",     Zone = "Balaka",     Region = "Southern", IsActive = true },
            new Department { Id = 19, Name = "Chikwawa Traffic",   Zone = "Chikwawa",   Region = "Southern", IsActive = true },
            new Department { Id = 20, Name = "Chiradzulu Traffic", Zone = "Chiradzulu", Region = "Southern", IsActive = true },
            new Department { Id = 21, Name = "Machinga Traffic",   Zone = "Machinga",   Region = "Southern", IsActive = true },
            new Department { Id = 22, Name = "Mangochi Traffic",   Zone = "Mangochi",   Region = "Southern", IsActive = true },
            new Department { Id = 23, Name = "Mulanje Traffic",    Zone = "Mulanje",    Region = "Southern", IsActive = true },
            new Department { Id = 24, Name = "Mwanza Traffic",     Zone = "Mwanza",     Region = "Southern", IsActive = true },
            new Department { Id = 25, Name = "Neno Traffic",       Zone = "Neno",       Region = "Southern", IsActive = true },
            new Department { Id = 26, Name = "Nsanje Traffic",     Zone = "Nsanje",     Region = "Southern", IsActive = true },
            new Department { Id = 27, Name = "Phalombe Traffic",   Zone = "Phalombe",   Region = "Southern", IsActive = true },
            new Department { Id = 28, Name = "Thyolo Traffic",     Zone = "Thyolo",     Region = "Southern", IsActive = true }
        );

        // Stations — one main police station per district (28 total)
        // existing 5 IDs preserved; 24 new ones added for the new districts.
        var seedDate = new DateTime(2026, 1, 1);
        mb.Entity<Station>().HasData(
            new Station { Id = 1,  Code = "STN-LIL-001", Name = "Area 18 Police Station",       DepartmentId = 1,  Zone = "Lilongwe",   PhysicalAddress = "Area 18, Lilongwe",       ContactPhone = "+265 1 750 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 2,  Code = "STN-LIL-002", Name = "Kamuzu Highway Checkpoint",    DepartmentId = 1,  Zone = "Lilongwe",   PhysicalAddress = "M1 Kamuzu Highway, North", ContactPhone = "+265 1 750 200", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 3,  Code = "STN-BLT-001", Name = "Limbe Police Station",         DepartmentId = 2,  Zone = "Blantyre",   PhysicalAddress = "Limbe, Blantyre",          ContactPhone = "+265 1 840 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 4,  Code = "STN-MZU-001", Name = "Mzuzu Central Police Station", DepartmentId = 3,  Zone = "Mzimba",     PhysicalAddress = "Mzuzu CBD",                ContactPhone = "+265 1 332 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 5,  Code = "STN-ZBA-001", Name = "Zomba Police Station",         DepartmentId = 4,  Zone = "Zomba",      PhysicalAddress = "Zomba CBD",                ContactPhone = "+265 1 525 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },

            // Northern (5 new)
            new Station { Id = 6,  Code = "STN-CHI-001", Name = "Chitipa Police Station",       DepartmentId = 5,  Zone = "Chitipa",    PhysicalAddress = "Chitipa Boma",             ContactPhone = "+265 1 382 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 7,  Code = "STN-KAR-001", Name = "Karonga Police Station",       DepartmentId = 6,  Zone = "Karonga",    PhysicalAddress = "Karonga Boma",             ContactPhone = "+265 1 362 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 8,  Code = "STN-LIK-001", Name = "Likoma Island Police Post",    DepartmentId = 7,  Zone = "Likoma",     PhysicalAddress = "Chizumulu Harbour",        ContactPhone = "+265 1 374 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 9,  Code = "STN-NKB-001", Name = "Nkhata Bay Police Station",    DepartmentId = 8,  Zone = "Nkhata Bay", PhysicalAddress = "Nkhata Bay Boma",          ContactPhone = "+265 1 352 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 10, Code = "STN-RUM-001", Name = "Rumphi Police Station",        DepartmentId = 9,  Zone = "Rumphi",     PhysicalAddress = "Rumphi Boma",              ContactPhone = "+265 1 372 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },

            // Central (8 new)
            new Station { Id = 11, Code = "STN-DED-001", Name = "Dedza Police Station",         DepartmentId = 10, Zone = "Dedza",      PhysicalAddress = "Dedza Boma, M1",           ContactPhone = "+265 1 223 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 12, Code = "STN-DOW-001", Name = "Dowa Police Station",          DepartmentId = 11, Zone = "Dowa",       PhysicalAddress = "Dowa Boma",                ContactPhone = "+265 1 282 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 13, Code = "STN-KAS-001", Name = "Kasungu Police Station",       DepartmentId = 12, Zone = "Kasungu",    PhysicalAddress = "Kasungu Boma",             ContactPhone = "+265 1 253 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 14, Code = "STN-MCH-001", Name = "Mchinji Border Police Station",DepartmentId = 13, Zone = "Mchinji",    PhysicalAddress = "Mchinji Border (Zambia)",  ContactPhone = "+265 1 242 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 15, Code = "STN-NKK-001", Name = "Nkhotakota Police Station",    DepartmentId = 14, Zone = "Nkhotakota", PhysicalAddress = "Nkhotakota Boma",          ContactPhone = "+265 1 292 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 16, Code = "STN-NTC-001", Name = "Ntcheu Police Station",        DepartmentId = 15, Zone = "Ntcheu",     PhysicalAddress = "Ntcheu Boma, M1",          ContactPhone = "+265 1 235 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 17, Code = "STN-NTI-001", Name = "Ntchisi Police Station",       DepartmentId = 16, Zone = "Ntchisi",    PhysicalAddress = "Ntchisi Boma",             ContactPhone = "+265 1 295 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 18, Code = "STN-SAL-001", Name = "Salima Police Station",        DepartmentId = 17, Zone = "Salima",     PhysicalAddress = "Salima Boma",              ContactPhone = "+265 1 263 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },

            // Southern (11 new)
            new Station { Id = 19, Code = "STN-BAL-001", Name = "Balaka Police Station",        DepartmentId = 18, Zone = "Balaka",     PhysicalAddress = "Balaka Boma",              ContactPhone = "+265 1 552 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 20, Code = "STN-CHK-001", Name = "Chikwawa Police Station",      DepartmentId = 19, Zone = "Chikwawa",   PhysicalAddress = "Chikwawa Boma",            ContactPhone = "+265 1 422 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 21, Code = "STN-CHZ-001", Name = "Chiradzulu Police Station",    DepartmentId = 20, Zone = "Chiradzulu", PhysicalAddress = "Chiradzulu Boma",          ContactPhone = "+265 1 462 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 22, Code = "STN-MAC-001", Name = "Machinga Police Station",      DepartmentId = 21, Zone = "Machinga",   PhysicalAddress = "Liwonde Boma",             ContactPhone = "+265 1 535 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 23, Code = "STN-MAN-001", Name = "Mangochi Police Station",      DepartmentId = 22, Zone = "Mangochi",   PhysicalAddress = "Mangochi Boma",            ContactPhone = "+265 1 584 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 24, Code = "STN-MUL-001", Name = "Mulanje Police Station",       DepartmentId = 23, Zone = "Mulanje",    PhysicalAddress = "Mulanje Boma",             ContactPhone = "+265 1 466 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 25, Code = "STN-MWA-001", Name = "Mwanza Border Police Station", DepartmentId = 24, Zone = "Mwanza",     PhysicalAddress = "Mwanza Border (Mozambique)",ContactPhone = "+265 1 432 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 26, Code = "STN-NEN-001", Name = "Neno Police Station",          DepartmentId = 25, Zone = "Neno",       PhysicalAddress = "Neno Boma",                ContactPhone = "+265 1 433 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 27, Code = "STN-NSA-001", Name = "Nsanje Police Station",        DepartmentId = 26, Zone = "Nsanje",     PhysicalAddress = "Nsanje Boma",              ContactPhone = "+265 1 451 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 28, Code = "STN-PHA-001", Name = "Phalombe Police Station",      DepartmentId = 27, Zone = "Phalombe",   PhysicalAddress = "Phalombe Boma",            ContactPhone = "+265 1 467 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
            new Station { Id = 29, Code = "STN-THY-001", Name = "Thyolo Police Station",        DepartmentId = 28, Zone = "Thyolo",     PhysicalAddress = "Thyolo Boma",              ContactPhone = "+265 1 473 100", IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
        );

        mb.Entity<PatrolPost>().HasData(
            new PatrolPost { Id = 1, Code = "PP-LIL-018-A", Name = "Kamuzu Hwy North",        StationId = 2, IsActive = true, CreatedAt = new DateTime(2026,1,1), UpdatedAt = new DateTime(2026,1,1) },
            new PatrolPost { Id = 2, Code = "PP-LIL-018-B", Name = "Kamuzu Hwy South",        StationId = 2, IsActive = true, CreatedAt = new DateTime(2026,1,1), UpdatedAt = new DateTime(2026,1,1) },
            new PatrolPost { Id = 3, Code = "PP-LIL-001-A", Name = "Area 18 Roundabout",      StationId = 1, IsActive = true, CreatedAt = new DateTime(2026,1,1), UpdatedAt = new DateTime(2026,1,1) },
            new PatrolPost { Id = 4, Code = "PP-BLT-001-A", Name = "Limbe Market Junction",   StationId = 3, IsActive = true, CreatedAt = new DateTime(2026,1,1), UpdatedAt = new DateTime(2026,1,1) },
            new PatrolPost { Id = 5, Code = "PP-MZU-001-A", Name = "Mzuzu Bus Depot",         StationId = 4, IsActive = true, CreatedAt = new DateTime(2026,1,1), UpdatedAt = new DateTime(2026,1,1) },
            new PatrolPost { Id = 6, Code = "PP-ZBA-001-A", Name = "Zomba M3 Junction",       StationId = 5, IsActive = true, CreatedAt = new DateTime(2026,1,1), UpdatedAt = new DateTime(2026,1,1) }
        );

        mb.Entity<OffenceCode>().HasData(
            new OffenceCode { Id = 1,  Code = "OC-001", Name = "Speeding",                         Description = "Exceeding the posted speed limit",                  DefaultFineAmount = 50000,  IsActive = true },
            new OffenceCode { Id = 2,  Code = "OC-002", Name = "Red Light Violation",               Description = "Failure to stop at a red traffic light",           DefaultFineAmount = 75000,  IsActive = true },
            new OffenceCode { Id = 3,  Code = "OC-003", Name = "No Seatbelt",                       Description = "Driver or passenger not wearing seatbelt",         DefaultFineAmount = 25000,  IsActive = true },
            new OffenceCode { Id = 4,  Code = "OC-004", Name = "Expired Road Licence",              Description = "Driving with an expired road licence",              DefaultFineAmount = 40000,  IsActive = true },
            new OffenceCode { Id = 5,  Code = "OC-005", Name = "Unroadworthy Vehicle",              Description = "Operating a vehicle in an unroadworthy condition",  DefaultFineAmount = 60000,  IsActive = true },
            new OffenceCode { Id = 6,  Code = "OC-006", Name = "Wrong Lane",                        Description = "Driving in the wrong lane",                        DefaultFineAmount = 30000,  IsActive = true },
            new OffenceCode { Id = 7,  Code = "OC-007", Name = "No Helmet",                         Description = "Motorcycle rider without a helmet",                DefaultFineAmount = 20000,  IsActive = true },
            new OffenceCode { Id = 8,  Code = "OC-008", Name = "Overloading",                       Description = "Vehicle carrying load exceeding permitted weight",  DefaultFineAmount = 80000,  IsActive = true },
            new OffenceCode { Id = 9,  Code = "OC-009", Name = "Driving Without Licence",           Description = "Operating a vehicle without a valid licence",       DefaultFineAmount = 100000, IsActive = true },
            new OffenceCode { Id = 10, Code = "OC-010", Name = "Use of Mobile Phone While Driving", Description = "Using a handheld mobile phone while driving",       DefaultFineAmount = 50000,  IsActive = true }
        );
    }
}
