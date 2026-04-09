using Microsoft.AspNetCore.Components.Web;
using NexusFine.Admin.Components;

var builder = WebApplication.CreateBuilder(args);

// ── SERVICES ──────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── HTTP CLIENT ───────────────────────────────────────────────
builder.Services.AddHttpClient("NexusFineAPI", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]
        ?? "http://localhost:5121"
    );
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("NexusFineAPI"));

// ── APP ───────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();