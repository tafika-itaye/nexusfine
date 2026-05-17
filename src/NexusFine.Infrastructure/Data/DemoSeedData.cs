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
        await SeedLilongwePilotVolumeAsync(db, logger);
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

    // ── PILOT-VOLUME LILONGWE SEED ───────────────────────────────────────
    //
    // Idempotent: keyed off the reference-number prefix "NXF-LIL-PIL-".
    // Generates ~500 realistic fines over the last 30 days across the two
    // Lilongwe pilot stations (Area 18 + Kamuzu Highway), six additional
    // Lilongwe officers, and the full GN-38/2026 offence catalogue.
    // Status mix, channel mix, time-of-day distribution and plate format
    // are all weighted to look like a real pilot week, not random noise.
    private static async Task SeedLilongwePilotVolumeAsync(AppDbContext db, ILogger? logger)
    {
        const string PilotRefPrefix = "NXF-LIL-PIL-";
        if (await db.Fines.AnyAsync(f => f.ReferenceNumber.StartsWith(PilotRefPrefix)))
        {
            logger?.LogInformation("Lilongwe pilot-volume already seeded — skipping.");
            return;
        }

        // ── 1. Top up Lilongwe officer roster (need at least 7 active) ──
        var existingBadges = await db.Officers.Select(o => o.BadgeNumber).ToListAsync();
        var newOfficers = new List<Officer>();

        (string badge, string first, string last, string rank, OfficerStatus status, int stationId, string loc, double lat, double lng)[] pilotRoster =
        {
            ("MPS-0101", "Yamikani", "Mhone",     "Constable",     OfficerStatus.Active,  1, "Area 18 Roundabout",        -13.9626, 33.7741),
            ("MPS-0102", "Thoko",    "Nyirenda",  "Constable",     OfficerStatus.Active,  1, "Capital Hill North gate",   -13.9701, 33.7795),
            ("MPS-0103", "Mphatso",  "Chirwa",    "Sergeant",      OfficerStatus.Active,  1, "Bingu Stadium roundabout",  -13.9844, 33.7672),
            ("MPS-0104", "Tiyamike", "Banda",     "Constable",     OfficerStatus.Active,  2, "Kamuzu Hwy N · Mile 6",     -13.8830, 33.7950),
            ("MPS-0105", "Ephraim",  "Gondwe",    "Constable",     OfficerStatus.OnBreak, 2, "Kamuzu Hwy S · Mile 4",     -14.0125, 33.7510),
            ("MPS-0106", "Annie",    "Kalua",     "Sub-Inspector", OfficerStatus.Active,  2, "Kanengo Industrial gate",   -13.9012, 33.8021),
        };

        var seedTime = DateTime.UtcNow;
        foreach (var r in pilotRoster)
        {
            if (existingBadges.Contains(r.badge)) continue;
            newOfficers.Add(new Officer
            {
                BadgeNumber       = r.badge,
                FirstName         = r.first,
                LastName          = r.last,
                Rank              = r.rank,
                Phone             = $"+26599{Random.Shared.Next(1000000, 9999999)}",
                Email             = $"{r.first.ToLower()}.{r.last.ToLower()}@mps.mw",
                DepartmentId      = 1,                // Lilongwe Traffic
                StationId         = r.stationId,
                NfcTagId          = $"NFC-{r.badge}",
                DeviceId          = $"DEV-{r.badge}",
                Status            = r.status,
                LastKnownLocation = r.loc,
                LastLatitude      = r.lat,
                LastLongitude     = r.lng,
                LastSyncAt        = seedTime.AddMinutes(-Random.Shared.Next(2, 25)),
                IsActive          = true,
            });
        }
        if (newOfficers.Count > 0)
        {
            db.Officers.AddRange(newOfficers);
            await db.SaveChangesAsync();
            logger?.LogInformation("Seeded {Count} additional Lilongwe pilot officers.", newOfficers.Count);
        }

        // ── 2. Pull everything we need for fine generation ──
        var lilongweOfficers = await db.Officers
            .Where(o => o.DepartmentId == 1 && o.IsActive)
            .ToListAsync();

        var offenceCodes = await db.OffenceCodes.Where(o => o.IsActive).ToListAsync();
        if (lilongweOfficers.Count == 0 || offenceCodes.Count == 0) return;

        // Weighted-offence picker — speeding most common, drunk driving rarer.
        // Sum must equal 100.
        (string code, int weight)[] offenceWeights =
        {
            ("OC-001", 28),   // Speeding
            ("OC-003", 14),   // No seatbelt
            ("OC-010", 10),   // Phone while driving
            ("OC-019",  8),   // Texting / calling while driving
            ("OC-002",  7),   // Failure to obey road signs
            ("OC-004",  6),   // Unregistered vehicle
            ("OC-007",  6),   // No helmet
            ("OC-006",  4),   // Dangerous overtaking
            ("OC-014",  4),   // No COF
            ("OC-005",  3),   // Unroadworthy vehicle
            ("OC-015",  3),   // Illegal parking
            ("OC-009",  2),   // No licence
            ("OC-012",  1),   // Drunk driving
            ("OC-013",  1),   // No insurance
            ("OC-016",  1),   // Failing to stop for officers
            ("OC-008",  1),   // Unsecured goods
            ("OC-011",  1),   // No plates
        };
        var weightedPicker = new List<OffenceCode>();
        foreach (var (code, weight) in offenceWeights)
        {
            var oc = offenceCodes.FirstOrDefault(o => o.Code == code);
            if (oc is null) continue;
            for (var i = 0; i < weight; i++) weightedPicker.Add(oc);
        }
        if (weightedPicker.Count == 0) weightedPicker = offenceCodes;

        // Channel mix for paid fines
        (PaymentChannel channel, int weight)[] channelWeights =
        {
            (PaymentChannel.AirtelMoney,  45),
            (PaymentChannel.TnmMpamba,    25),
            (PaymentChannel.Cash,         15),
            (PaymentChannel.BankTransfer,  7),
            (PaymentChannel.Ussd,          4),
            (PaymentChannel.Card,          3),
            (PaymentChannel.WhatsApp,      1),
        };
        var channelPicker = new List<PaymentChannel>();
        foreach (var (ch, w) in channelWeights)
            for (var i = 0; i < w; i++) channelPicker.Add(ch);

        // Realistic Malawian driver names + plate prefixes + vehicle catalogue
        string[] firstNames =
        {
            "Chimwemwe","Tadala","Yamikani","Mphatso","Tiyamike","Thandiwe","Annie","Grace",
            "Patrick","Joseph","Mary","Esther","James","Peter","David","Daniel","Limbani",
            "Mwayi","Khumbo","Madalitso","Chisomo","Ulemu","Kondwani","Pemphero","Loveness",
            "Benson","Rodgers","Eunice","Tamanda","Ireen","Hilda","Memory","Blessings","Fred",
        };
        string[] surnames =
        {
            "Banda","Phiri","Mwale","Chirwa","Mvula","Nyirenda","Tembo","Kalua","Mhone",
            "Gondwe","Mtonga","Chimombo","Kachale","Lungu","Kanyenda","Chilembwe","Sambo",
            "Manda","Kaunda","Chiponda","Mhango","Ng'ambi","Munthali","Soko","Mbewe",
        };
        string[] vehicleMakes = { "Toyota","Toyota","Toyota","Nissan","Nissan","Honda","Mazda","Mitsubishi","Isuzu","Suzuki" };
        string[] vehicleModels =
        {
            "Hilux","Corolla","Vitz","Land Cruiser","Hardbody","NP200","Civic","CX-5","Pajero","Alto","Swift","D-Max"
        };
        string[] vehicleColours = { "White","White","White","Silver","Silver","Black","Grey","Blue","Red","Maroon","Green","Beige" };

        string[] lilongweRoads =
        {
            "Kenyatta Drive · City Centre",
            "Kamuzu Highway · Mile 4",
            "Kamuzu Highway · Mile 6 (Kanengo)",
            "Presidential Way · Capital Hill",
            "Area 18 Roundabout",
            "M1 north · Mchinji turnoff",
            "M1 south · Lilongwe Bridge",
            "Paul Kagame Road · Falls Estate",
            "Mzimba Street · Old Town",
            "Mchinji Road · Area 25",
            "Bingu Stadium roundabout",
            "Cross Roads · Area 13",
            "Lilongwe Bypass · Lumbadzi",
            "Capital Hill exit · Statehouse Road",
            "Salima Road · Area 47",
        };
        var lilongwePlateSeries = new[] { "MW-LL-" };

        // ── 3. Generate ~500 fines distributed over the last 30 days ──
        const int targetFineCount = 500;
        var rng = new Random(20260516);
        var pilotStart = DateTime.UtcNow.AddDays(-30);

        var fines = new List<Fine>(capacity: targetFineCount);
        for (var i = 1; i <= targetFineCount; i++)
        {
            var officer = lilongweOfficers[rng.Next(lilongweOfficers.Count)];
            var offence = weightedPicker[rng.Next(weightedPicker.Count)];

            // Hour distribution biased to commute hours (06–09, 15–19)
            int hour;
            var roll = rng.NextDouble();
            if      (roll < 0.30) hour = rng.Next(6, 9);
            else if (roll < 0.65) hour = rng.Next(15, 19);
            else                  hour = rng.Next(0, 24);
            var issuedAt = pilotStart
                .AddDays(rng.NextDouble() * 30)
                .Date
                .AddHours(hour)
                .AddMinutes(rng.Next(0, 60))
                .AddSeconds(rng.Next(0, 60));

            // Status mix: 65% Paid, 18% Unpaid, 10% Overdue, 5% Disputed, 2% Cancelled
            var statusRoll = rng.NextDouble();
            FineStatus status;
            if      (statusRoll < 0.65) status = FineStatus.Paid;
            else if (statusRoll < 0.83) status = FineStatus.Unpaid;
            else if (statusRoll < 0.93) status = FineStatus.Overdue;
            else if (statusRoll < 0.98) status = FineStatus.Disputed;
            else                        status = FineStatus.Cancelled;

            var firstName = firstNames[rng.Next(firstNames.Length)];
            var lastName  = surnames[rng.Next(surnames.Length)];
            var plate     = $"{lilongwePlateSeries[0]}{rng.Next(1000, 9999)}";

            fines.Add(new Fine
            {
                ReferenceNumber     = $"{PilotRefPrefix}{i:D5}",
                IssuedAt            = issuedAt,
                DueDate             = issuedAt.AddDays(30),
                PlateNumber         = plate,
                VehicleMake         = vehicleMakes[rng.Next(vehicleMakes.Length)],
                VehicleModel        = vehicleModels[rng.Next(vehicleModels.Length)],
                VehicleColour       = vehicleColours[rng.Next(vehicleColours.Length)],
                DriverName          = $"{firstName} {lastName}",
                DriverNationalId    = $"NID-LL-{rng.Next(100_000, 999_999)}",
                DriverLicenceNumber = $"DL-LL-{rng.Next(100_000, 999_999)}",
                DriverPhone         = $"+26599{rng.Next(1_000_000, 9_999_999)}",
                OffenceCodeId       = offence.Id,
                Location            = lilongweRoads[rng.Next(lilongweRoads.Length)],
                Latitude            = -13.96 + (rng.NextDouble() - 0.5) * 0.15,
                Longitude            =  33.78 + (rng.NextDouble() - 0.5) * 0.15,
                Amount              = offence.DefaultFineAmount,
                PenaltyAmount       = status == FineStatus.Overdue
                                        ? offence.DefaultFineAmount * 0.10m
                                        : 0,
                Status              = status,
                OfficerId           = officer.Id,
                DepartmentId        = 1,            // Lilongwe Traffic
                Notes               = $"Pilot-seed · day {(int)(issuedAt - pilotStart).TotalDays + 1}",
            });
        }

        db.Fines.AddRange(fines);
        await db.SaveChangesAsync();
        logger?.LogInformation("Seeded {Count} pilot-volume Lilongwe fines.", fines.Count);

        // ── 4. Payments for the Paid ones, with realistic channel mix ──
        var paid = fines.Where(f => f.Status == FineStatus.Paid).ToList();
        var payments = new List<Payment>(paid.Count);
        var receiptCounter = 0;
        foreach (var f in paid)
        {
            receiptCounter++;
            var channel = channelPicker[rng.Next(channelPicker.Count)];
            // Citizens pay anywhere from 5 min to 5 days later (mode ~6 hours)
            var payLagMinutes = (int)(rng.NextDouble() * rng.NextDouble() * 7200);
            var initiatedAt = f.IssuedAt.AddMinutes(payLagMinutes + 5);
            var completedAt = initiatedAt.AddMinutes(rng.Next(1, 8));

            payments.Add(new Payment
            {
                FineId               = f.Id,
                ReceiptNumber        = $"MPAY-LIL-PIL-{receiptCounter:D5}",
                Amount               = f.Amount + (f.PenaltyAmount ?? 0),
                Channel              = channel,
                Status               = PaymentStatus.Completed,
                PhoneNumber          = f.DriverPhone,
                TransactionReference = $"SIM-{channel.ToString().ToUpper()[..3]}-{f.ReferenceNumber}",
                InitiatedAt          = initiatedAt,
                CompletedAt          = completedAt,
            });
        }
        db.Payments.AddRange(payments);
        await db.SaveChangesAsync();
        logger?.LogInformation("Seeded {Count} pilot-volume payments.", payments.Count);

        // ── 5. A handful of disputes for the Disputed fines (≤ 5 newest) ──
        var disputed = fines.Where(f => f.Status == FineStatus.Disputed)
                            .OrderByDescending(f => f.IssuedAt)
                            .Take(5).ToList();
        var appeals = new List<Appeal>();
        var disputeReasons = new[]
        {
            "I was not the driver at the time — vehicle was on hire.",
            "Plate is mis-read; my plate is one digit different.",
            "Speed camera was facing wrong lane.",
            "Officer did not display ID; request review of bodycam.",
            "Same offence already paid — duplicate ticket.",
        };
        var appealCounter = 0;
        foreach (var f in disputed)
        {
            appealCounter++;
            appeals.Add(new Appeal
            {
                ReferenceNumber  = $"APP-2026-PIL-{appealCounter:D4}",
                FineId           = f.Id,
                Reason           = disputeReasons[rng.Next(disputeReasons.Length)],
                ApplicantName    = f.DriverName,
                ApplicantPhone   = f.DriverPhone ?? "+265991234567",
                Status           = AppealStatus.Submitted,
                SubmittedAt      = f.IssuedAt.AddDays(rng.Next(1, 7)),
            });
        }
        db.Appeals.AddRange(appeals);
        await db.SaveChangesAsync();
        logger?.LogInformation("Seeded {Count} pilot appeals.", appeals.Count);
    }
}
