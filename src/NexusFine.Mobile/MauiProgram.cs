using Microsoft.Extensions.Logging;
using NexusFine.Mobile.Services;
using NexusFine.Mobile.Pages;

namespace NexusFine.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf",    "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf",   "OpenSansSemibold");
            });

        // ── HTTP CLIENT ───────────────────────────────────────
        builder.Services.AddHttpClient("NexusFineAPI", client =>
        {
            // Change to your server IP when deploying to a real device
            client.BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5121/"   // Android emulator → localhost
                : "http://localhost:5121/"); // iOS simulator / Windows
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // ── SERVICES ──────────────────────────────────────────
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<SessionService>();

        // ── PAGES ─────────────────────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<IssuFinePage>();
        builder.Services.AddTransient<MyFinesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
