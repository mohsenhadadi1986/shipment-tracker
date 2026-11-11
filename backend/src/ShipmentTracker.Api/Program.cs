using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Infrastructure;
using ShipmentTracker.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Shipment Tracker API",
        Version = "v1",
        Description = "Step 1: foundation only — entities + EF Core + health. CRUD arrives in step 2."
    });
});

// Registers DbContext + SQL Server (see Infrastructure/DependencyInjection.cs)
builder.Services.AddInfrastructure(builder.Configuration);

// Health checks: process is up + can talk to SQL Server via EF
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ShipmentTrackerDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Skip HTTPS redirect in Development so http://localhost:5080 works without a cert dance.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new
{
    service = "Shipment Tracker API",
    step = 1,
    message = "Foundation ready. CRUD controllers arrive in step 2.",
    docs = "/swagger"
}));

app.Run();

// Expose for WebApplicationFactory in later test steps
public partial class Program;
