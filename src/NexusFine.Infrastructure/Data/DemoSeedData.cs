using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Auth;

namespace NexusFine.Infrastructure.Data;

/// <summary>
/// Runtime seeder. Idempotent — safe to run on every startup. Inserts:
///   • Admin + supervisor accounts
///   • A small roster of officers, one per zone
///   • A handful of recent sample fines so the Minister demo dashboards
///     are not empty the first time the app is launched.
/// </summary>
public static class DemoSeedData
{
    public static async Task SeedAsync(AppDbContext db, PasswordHasher hasher, ILogger? logger = null)
    {
        await SeedUsersAsync(db, hasher, logger);
        await SeedOfficersAsync(db, logger);
        await SeedSampleFinesAsync(db, logger);
    }

    private static async Task SeedUsersAsync(AppDbContext db, PasswordHasher hasher, ILogger? logger)
    {
        if (await db.AppUsers.AnyAsync()) return;

        var admin = hasher.Hash("Nexus@Admin2026");
        var super = hasher.Hash("Nexus@Super2026");

        db.AppUsers.AddRange(
            new AppUser
            {
                UserName     = "admin",
                Email        = "admin@nexusfine.mw",
                FullName     = "System Administrator",
                PasswordHash = admin.Hash,
                PasswordSalt = admin.Salt,
                Roles        = AppRoles.Admin,
                IsActive     = true
            },
            new AppUser
            {
                UserName     = "supervisor",
                Email        = "supervisor@nexusfine.mw",
                FullName     = "Lilongwe Supervisor",
                PasswordHash = super.Hash,
                PasswordSalt = super.Salt,
                Roles        = $"{AppRoles.Supervisor},{AppRoles.Admin}",
                IsActive     = true
            });

        await db.SaveChangesAsync();
        logger?.LogInformation("Seeded default admin + supervisor users.");
    }

    private static async Task SeedOfficersAsync(AppDbContext db, ILogger? logger)
    {
        if (await db.Officers.AnyAsync()) return;

        db.Officers.AddRange(
            new Officer
            {
                BadgeNumber       = "MPS-0001",
                FirstName         = "Chisomo",
                LastName          = "Banda",
                Rank              = "Sergeant",
                Phone             = "+265991112233",
                Email             = "c.banda@mps.mw",
                DepartmentId      = 1,
                NfcTagId          = "NFC-MPS-0001",
                DeviceId          = "DEV-0001",
                Status            = OfficerStatus.Active,
                LastKnownLocation = "City Centre Roundabout",
                LastLatitude      = -13.9626,
                LastLongitude     = 33.7741,
                LastSyncAt        = DateTime.UtcNow.AddMinutes(-3),
                IsActive          = true
            },
            new Officer
            {
                BadgeNumber       = "MPS-0002",
                FirstName         = "Tamanda",
                LastName          = "Phiri",
                Rank              = "Constable",
                Phone             = "+265991112244",
                Email             = "t.phiri@mps.mw",
                DepartmentId      = 2,
                NfcTagId          = "NFC-MPS-0002",
                DeviceId          = "DEV-0002",
                Status            = OfficerStatus.Active,
                LastKnownLocation = "Chichiri Roundabout",
                LastLatitude      = -15.7905,
                LastLongitude     = 35.0050,
                LastSyncAt        = DateTime.UtcNow.AddMinutes(-7),
                IsActive          = true
            },
            new Officer
            {
                BadgeNumber       = "MPS-0003",
                FirstName         = "Blessings",
                LastName          = "Mwale",
                Rank              = "Inspector",
                Phone             = "+265991112255",
                Email             = "b.mwale@mps.mw",
                DepartmentId      = 3,
                NfcTagId          = "NFC-MPS-0003",
                DeviceId          = "DEV-0003",
                Status            = OfficerStatus.OnBreak,
                LastKnownLocation = "Katoto Roundabout",
                LastLatitude      = -11.4607,
                LastLongitude     = 34.0207,
                LastSyncAt        = DateTime.UtcNow.AddMinutes(-12),
                IsActive          = true
            },
            new Officer
            {
                BadgeNumber       = "MPS-0004",
                FirstName         = "Grace",
                LastName          = "Kachale",
                Rank              = "Constable",
                Phone             = "+265991112266",
                Email             = "g.kachale@mps.mw",
                DepartmentId      = 4,
                NfcTagId          = "NFC-MPS-0004",
                DeviceId          = "DEV-0004",
                Status            = OfficerStatus.Active,
                LastKnownLocation = "Zomba Main Road",
                LastLatitude      = -15.3820,
                LastLongitude     = 35.3200,
                LastSyncAt        = DateTime.UtcNow.AddMinutes(-2),
                IsActive          = true
            });

        await db.SaveChangesAsync();
        logger?.LogInformation("Seeded 4 demo officers (one per zone).");
    }

    private static async Task SeedSampleFinesAsync(AppDbContext db, ILogger? logger)
    {
        if (await db.Fines.AnyAsync()) return;

        var officers = await db.Officers.ToListAsync();
        var offences = await db.OffenceCodes.ToListAsync();
        if (officers.Count == 0 || offences.Count == 0) return;

        var rng = new Random(20260424);
        var fines = new List<Fine>();
        var plates = new[]
        {
            "MW-BA-1234", "MW-LA-5678", "MW-BZ-9012", "MW-MZ-3344",
            "MW-LA-7788", "MW-BA-2020", "MW-ZA-4455", "MW-BA-8899"
        };

        for (var i = 0; i < plates.Length; i++)
        {
            var officer   = officers[rng.Next(officers.Count)];
            var offence   = offences[rng.Next(offences.Count)];
            var issuedAt  = DateTime.UtcNow.AddDays(-rng.Next(0, 14))
                                           .AddHours(-rng.Next(0, 10));
            var status    = (i % 3) switch
            {
                0 => FineStatus.Paid,
                1 => FineStatus.Unpaid,
                _ => FineStatus.Overdue
            };

            fines.Add(new Fine
            {
                ReferenceNumber     = $"NXF-2026-{(i + 1):D5}",
                IssuedAt            = issuedAt,
                DueDate             = issuedAt.AddDays(30),
                PlateNumber         = plates[i],
                VehicleMake         = i % 2 == 0 ? "Toyota"  : "Nissan",
                VehicleModel        = i % 2 == 0 ? "Hilux"   : "Hardbody",
                VehicleColour       = i % 2 == 0 ? "White"   : "Silver",
                DriverName          = $"Demo Driver {i + 1}",
                DriverNationalId    = $"NID-{100000 + i}",
                DriverLicenceNumber = $"DL-{200000 + i}",
                DriverPhone         = $"+26599100{1000 + i}",
                OffenceCodeId       = offence.Id,
                Location            = officer.LastKnownLocation ?? "Unknown",
                Latitude            = officer.LastLatitude,
                Longitude           = officer.LastLongitude,
                Amount              = offence.DefaultFineAmount,
                PenaltyAmount       = status == FineStatus.Overdue
                                         ? offence.DefaultFineAmount * 0.10m
                                         : 0,
                Status              = status,
                OfficerId           = officer.Id,
                DepartmentId        = officer.DepartmentId
            });
        }

        db.Fines.AddRange(fines);
        await db.SaveChangesAsync();

        // Attach payments for the Paid ones
        var paid = fines.Where(f => f.Status == FineStatus.Paid).ToList();
        foreach (var f in paid)
        {
            db.Payments.Add(new Payment
            {
                FineId               = f.Id,
                ReceiptNumber        = $"MPAY-2026-{f.Id:D5}",
                Amount               = f.Amount,
                Channel              = PaymentChannel.AirtelMoney,
                Status               = PaymentStatus.Completed,
                PhoneNumber          = f.DriverPhone,
                TransactionReference = $"SIM-{f.ReferenceNumber}",
                InitiatedAt          = f.IssuedAt.AddHours(2),
                CompletedAt          = f.IssuedAt.AddHours(2).AddMinutes(3)
            });
        }

        await db.SaveChangesAsync();
        logger?.LogInformation("Seeded {Count} sample fines.", fines.Count);
    }
}
