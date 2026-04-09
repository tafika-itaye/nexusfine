using Microsoft.Extensions.DependencyInjection;
using NexusFine.Core.Entities;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// Resolves the correct payment gateway implementation
/// based on the PaymentChannel selected by the citizen.
/// </summary>
public class PaymentGatewayFactory
{
    private readonly IServiceProvider _services;

    public PaymentGatewayFactory(IServiceProvider services)
    {
        _services = services;
    }

    public IPaymentGateway GetGateway(PaymentChannel channel)
    {
        return channel switch
        {
            PaymentChannel.AirtelMoney => _services.GetRequiredService<AirtelMoneyService>(),
            PaymentChannel.TnmMpamba  => _services.GetRequiredService<MpambaService>(),
            _ => throw new NotSupportedException(
                $"No gateway configured for channel: {channel}. " +
                $"Supported channels: AirtelMoney, TnmMpamba.")
        };
    }
}
