using Microsoft.EntityFrameworkCore;
using NexusFine.Core.Entities;
using NexusFine.Infrastructure.Data;
using Xunit;

namespace NexusFine.Tests;

/// <summary>
/// Exercises the fine → payment → paid flow against the in-memory provider.
/// This is a smoke test for the entity relationships, not a controller test.
/// </summary>
public class FineLifecycleTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"nexusfine-{Guid.NewGuid()}")
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task CreateFine_DefaultsToUnpaid()
    {
        await using var db = NewDb();
        db.Departments.Add(new Department  { Id = 1, Name = "Test", Zone = "Zone", IsActive = true });
        db.OffenceCodes.Add(new OffenceCode { Id = 1, Code = "OC-T", Name = "Test", DefaultFineAmount = 10000m });
        db.Officers.Add(new Officer
        {
            Id = 1, BadgeNumber = "MPS-T", FirstName = "T", LastName = "T", Rank = "T",
            DepartmentId = 1, Status = OfficerStatus.Active, IsActive = true
        });
        await db.SaveChangesAsync();

        var fine = new Fine
        {
            ReferenceNumber = "NXF-2026-99999",
            PlateNumber     = "MW-T-0001",
            DriverName      = "Test Driver",
            OffenceCodeId   = 1,
            OfficerId       = 1,
            DepartmentId    = 1,
            Location        = "Unit Test",
            IssuedAt        = DateTime.UtcNow,
            DueDate         = DateTime.UtcNow.AddDays(30),
            Amount          = 10000m
        };
        db.Fines.Add(fine);
        await db.SaveChangesAsync();

        var saved = await db.Fines.FirstAsync(f => f.ReferenceNumber == "NXF-2026-99999");
        Assert.Equal(FineStatus.Unpaid, saved.Status);
    }

    [Fact]
    public async Task CompletingPayment_MovesFineToPaid()
    {
        await using var db = NewDb();
        db.Departments.Add(new Department  { Id = 1, Name = "Test", Zone = "Zone", IsActive = true });
        db.OffenceCodes.Add(new OffenceCode { Id = 1, Code = "OC-T", Name = "Test", DefaultFineAmount = 10000m });
        db.Officers.Add(new Officer
        {
            Id = 1, BadgeNumber = "MPS-T", FirstName = "T", LastName = "T", Rank = "T",
            DepartmentId = 1, Status = OfficerStatus.Active, IsActive = true
        });
        var fine = new Fine
        {
            ReferenceNumber = "NXF-2026-00002",
            PlateNumber     = "MW-T-0002",
            DriverName      = "Driver",
            OffenceCodeId   = 1,
            OfficerId       = 1,
            DepartmentId    = 1,
            Location        = "Unit Test",
            IssuedAt        = DateTime.UtcNow,
            DueDate         = DateTime.UtcNow.AddDays(30),
            Amount          = 10000m,
            Status          = FineStatus.Unpaid
        };
        db.Fines.Add(fine);
        await db.SaveChangesAsync();

        var payment = new Payment
        {
            FineId               = fine.Id,
            ReceiptNumber        = "MPAY-2026-00002",
            Amount               = 10000m,
            Channel              = PaymentChannel.AirtelMoney,
            Status               = PaymentStatus.Completed,
            TransactionReference = "SIM-XYZ",
            CompletedAt          = DateTime.UtcNow
        };
        db.Payments.Add(payment);
        fine.Status = FineStatus.Paid;
        await db.SaveChangesAsync();

        var saved = await db.Fines
            .Include(f => f.Payments)
            .FirstAsync(f => f.Id == fine.Id);

        Assert.Equal(FineStatus.Paid, saved.Status);
        Assert.Single(saved.Payments);
        Assert.Equal(PaymentStatus.Completed, saved.Payments.First().Status);
    }
}
