using Microsoft.AspNetCore.Components.Authorization;
using NexusFine.Admin.Components;
using NexusFine.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// ── BLAZOR SERVER ────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ── AUTH (JWT held in circuit memory) ────────────────────────
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

// DelegatingHandler that injects "Authorization: Bearer …" into every API call.
builder.Services.AddTransient<JwtBearerHandler>();

// ── HTTP CLIENT (talks to the NexusFine API) ────────────────
builder.Services.AddHttpClient("NexusFineAPI", client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["ApiSettings:BaseUrl"]
            ?? "http://localhost:5121"
        );
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddHttpMessageHandler<JwtBearerHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("NexusFineAPI"));

// ── APP ──────────────────────────────────────────────────────
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
