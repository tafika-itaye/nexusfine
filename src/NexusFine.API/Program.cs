using Microsoft.EntityFrameworkCore;
using NexusFine.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ── SERVICES ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// ── DATABASE ──────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("NexusFine.Infrastructure")
    )
);

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("NexusFinePolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
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
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("NexusFinePolicy");
app.UseAuthorization();
app.MapControllers();

// ── AUTO-MIGRATE ON STARTUP (dev only) ───────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();