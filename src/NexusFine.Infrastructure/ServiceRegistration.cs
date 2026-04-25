using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusFine.Infrastructure.Auth;
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
        // ── AUTH ──────────────────────────────────────────────
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddSingleton<JwtTokenService>();
        services.AddSingleton<PasswordHasher>();

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

        services.AddHttpClient("WhatsApp", client =>
        {
            client.BaseAddress = new Uri(
                config["WhatsApp:BaseUrl"] ?? "https://graph.facebook.com/v20.0/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(15);
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
        services.AddScoped<SimulatedPaymentGateway>();
        services.AddScoped<PaymentGatewayFactory>();

        // ── NOTIFICATIONS ─────────────────────────────────────
        // WhatsApp is the primary channel for the DRTSS pilot. SMS and Email
        // are retained as OPTIONAL features — any tenant can enable them in
        // config to reach citizens on additional channels. The CompositeNotificationService
        // fans out to all enabled channels with isolated failure handling.
        services.AddScoped<WhatsAppNotificationService>();
        services.AddScoped<SmsNotificationService>();
        services.AddScoped<EmailNotificationService>();
        services.AddScoped<INotificationService, CompositeNotificationService>();

        return services;
    }
}
