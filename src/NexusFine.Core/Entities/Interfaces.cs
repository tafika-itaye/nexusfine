using NexusFine.Core.Entities;

namespace NexusFine.Core.Interfaces;

public interface IFineRepository
{
    Task<Fine?> GetByIdAsync(int id);
    Task<Fine?> GetByReferenceAsync(string reference);
    Task<Fine?> GetByPlateAsync(string plate);
    Task<Fine?> GetByDriverIdAsync(string nationalId);
    Task<IEnumerable<Fine>> GetByOfficerAsync(int officerId, DateTime? date = null);
    Task<IEnumerable<Fine>> GetByDepartmentAsync(int departmentId, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<Fine>> GetOverdueAsync();
    Task<Fine> CreateAsync(Fine fine);
    Task<Fine> UpdateAsync(Fine fine);
    Task<int> CountTodayAsync();
}

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<Payment?> GetByReceiptAsync(string receipt);
    Task<IEnumerable<Payment>> GetByFineAsync(int fineId);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
    Task<decimal> GetTotalCollectedAsync(DateTime from, DateTime to);
}

public interface IOfficerRepository
{
    Task<Officer?> GetByIdAsync(int id);
    Task<Officer?> GetByBadgeAsync(string badge);
    Task<IEnumerable<Officer>> GetAllActiveAsync();
    Task<IEnumerable<Officer>> GetByDepartmentAsync(int departmentId);
    Task<Officer> UpdateAsync(Officer officer);
    Task UpdateLocationAsync(int officerId, double lat, double lng, string locationName);
}

public interface IPaymentService
{
    Task<PaymentInitiationResult> InitiateAsync(int fineId, PaymentChannel channel, string? phone);
    Task<bool> VerifyAsync(string transactionReference);
    Task<bool> HandleCallbackAsync(string gatewayPayload);
}

public record PaymentInitiationResult(
    bool Success,
    string? ReceiptNumber,
    string? TransactionReference,
    string? Message
);
