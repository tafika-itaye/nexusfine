using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication;
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

// Required by ASP.NET Core middleware even though Blazor handles auth internally
builder.Services.AddAuthentication("NoOp")
    .AddScheme<AuthenticationSchemeOptions, NoOpAuthHandler>("NoOp", _ => { });

// ── HTTP CLIENT (talks to the NexusFine API) ────────────────
// Note: we deliberately bypass IHttpClientFactory here. HttpClientFactory
// builds the message-handler chain in its OWN internal DI scope (see
// https://learn.microsoft.com/aspnet/core/fundamentals/http-requests
// #message-handler-scopes-in-ihttpclientfactory) — that scope is NOT the
// SignalR circuit's scope. The result is that JwtBearerHandler ends up
// holding a JwtAuthStateProvider instance from a different scope, so the
// token signed in on the circuit's provider is never seen and every API
// call comes back 401.
//
// For this admin portal (one tenant, modest traffic) we trade
// HttpClientFactory's connection pooling for correctness: we register the
// HttpClient as Scoped and construct it directly with a circuit-scoped
// JwtBearerHandler that reads from the *same* JwtAuthStateProvider the
// Login page wrote to.
builder.Services.AddScoped<HttpClient>(sp =>
{
    var auth   = sp.GetRequiredService<JwtAuthStateProvider>();
    var config = sp.GetRequiredService<IConfiguration>();
    var handler = new JwtBearerHandler(auth)
    {
        InnerHandler = new HttpClientHandler()
    };
    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri(
            config["ApiSettings:BaseUrl"] ?? "http://localhost:5121"),
    };
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    return client;
});

// ── APP ──────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
