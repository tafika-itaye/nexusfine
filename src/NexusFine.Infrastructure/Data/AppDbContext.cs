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
            e.Property(d => d.Name).HasMaxLength(100).IsRequired();
            e.Property(d => d.Zone).HasMaxLength(60).IsRequired();
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

        // ── SEED DATA ──
        mb.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Lilongwe Traffic",  Zone = "Lilongwe", IsActive = true },
            new Department { Id = 2, Name = "Blantyre Traffic",  Zone = "Blantyre",  IsActive = true },
            new Department { Id = 3, Name = "Mzuzu Traffic",     Zone = "Mzuzu",     IsActive = true },
            new Department { Id = 4, Name = "Zomba Traffic",     Zone = "Zomba",     IsActive = true }
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
