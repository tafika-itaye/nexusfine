using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusFine.Core.Entities;

namespace NexusFine.Infrastructure.Services;

/// <summary>
/// Resolves the correct payment gateway implementation based on:
///   1. ApiSettings:PaymentMode — "Simulated" forces SimulatedPaymentGateway.
///   2. Otherwise by PaymentChannel (AirtelMoney, TnmMpamba).
///
/// For the Minister demo we run in Simulated mode; flipping the config key
/// to "Live" routes traffic to the real gateways — no code change required.
/// </summary>
public class PaymentGatewayFactory
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration   _config;

    public PaymentGatewayFactory(IServiceProvider services, IConfiguration config)
    {
        _services = services;
        _config   = config;
    }

    public IPaymentGateway GetGateway(PaymentChannel channel)
    {
        var mode = _config["ApiSettings:PaymentMode"] ?? "Simulated";

        if (mode.Equals("Simulated", StringComparison.OrdinalIgnoreCase))
            return _services.GetRequiredService<SimulatedPaymentGateway>();

        return channel switch
        {
            PaymentChannel.AirtelMoney => _services.GetRequiredService<AirtelMoneyService>(),
            PaymentChannel.TnmMpamba   => _services.GetRequiredService<MpambaService>(),
            _ => throw new NotSupportedException(
                $"No live gateway configured for channel: {channel}. " +
                $"Supported live channels: AirtelMoney, TnmMpamba.")
        };
    }
}
