using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NexusFine.API.Middleware;
using NexusFine.Infrastructure;
using NexusFine.Infrastructure.Auth;
using NexusFine.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ── CORE SERVICES ─────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with Bearer scheme so ops can auth through the portal.
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "NexusFine API",
        Version = "v1",
        Description = "Digital traffic fines management — Malawi Police Service / DRTSS."
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Paste the JWT access token (no 'Bearer ' prefix)."
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── DATABASE ──────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("NexusFine.Infrastructure")
    )
);

// ── INFRASTRUCTURE (auth, payment gateways, notifications) ───
builder.Services.AddNexusFineInfrastructure(builder.Configuration);

// ── JWT AUTH ─────────────────────────────────────────────────
// Bind Jwt settings directly so we can build validation parameters without
// spinning up a temporary service provider.
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtConfig  = jwtSection.Get<JwtSettings>()
                 ?? throw new InvalidOperationException("Jwt settings are missing from configuration.");

if (string.IsNullOrWhiteSpace(jwtConfig.Secret) || jwtConfig.Secret.Length < 32)
    throw new InvalidOperationException("Jwt:Secret must be configured and at least 32 characters long.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtConfig.Issuer,
            ValidAudience            = jwtConfig.Audience,
            IssuerSigningKey         = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtConfig.Secret)),
            ClockSkew                = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("NexusFinePolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5000",
                "https://tafika-itaye.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── APP ───────────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "NexusFine API v1");
        opt.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("NexusFinePolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseNexusFineAuditLog();

app.MapControllers();

// ── AUTO-MIGRATE + SEED ON STARTUP (dev only) ────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
    var log    = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                     .CreateLogger("DemoSeed");

    db.Database.Migrate();
    await DemoSeedData.SeedAsync(db, hasher, log);
}

app.Run();
