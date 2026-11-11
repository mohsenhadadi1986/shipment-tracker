using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipmentTracker.Infrastructure.Persistence;

namespace ShipmentTracker.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core DbContext and SQL Server provider.
    /// Called from Program.cs so the Api project stays thin.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing. Check appsettings.json / user secrets / environment.");

        services.AddDbContext<ShipmentTrackerDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
