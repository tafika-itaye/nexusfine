using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusFine.Infrastructure.Services;

namespace NexusFine.Infrastructure;

/// <summary>
/// Extension method to register all Infrastructure services.
/// Call this from Program.cs: builder.Services.AddNexusFineInfrastructure(config)
/// </summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddNexusFineInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // ── HTTP CLIENTS ──────────────────────────────────────
        services.AddHttpClient("AirtelMoney", client =>
        {
            client.BaseAddress = new Uri(
                config["AirtelMoney:BaseUrl"] ?? "https://openapi.airtel.africa/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("TnmMpamba", client =>
        {
            client.BaseAddress = new Uri(
                config["TnmMpamba:BaseUrl"] ?? "https://api.tnmmobile.com/v1/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("SmsGateway", client =>
        {
            client.BaseAddress = new Uri(
                config["Sms:BaseUrl"] ?? "https://api.africastalking.com/version1/messaging/");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        // ── PAYMENT GATEWAYS ──────────────────────────────────
        services.AddScoped<AirtelMoneyService>();
        services.AddScoped<MpambaService>();
        services.AddScoped<PaymentGatewayFactory>();

        // ── SMS ───────────────────────────────────────────────
        services.AddScoped<SmsService>();

        return services;
    }
}
