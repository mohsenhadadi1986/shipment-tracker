using Microsoft.EntityFrameworkCore;
using ShipmentTracker.Domain.Entities;

namespace ShipmentTracker.Infrastructure.Persistence;

public class ShipmentTrackerDbContext : DbContext
{
    public ShipmentTrackerDbContext(DbContextOptions<ShipmentTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Port> Ports => Set<Port>();
    public DbSet<Vessel> Vessels => Set<Vessel>();
    public DbSet<Container> Containers => Set<Container>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentContainer> ShipmentContainers => Set<ShipmentContainer>();
    public DbSet<TrackingEvent> TrackingEvents => Set<TrackingEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Discover all IEntityTypeConfiguration<T> in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShipmentTrackerDbContext).Assembly);

        // Seed via HasData so the initial migration includes INSERT statements.
        // See docs/step-01.md for why HasData vs a runtime seeder.
        modelBuilder.Entity<Port>().HasData(SeedData.Ports);
        modelBuilder.Entity<Vessel>().HasData(SeedData.Vessels);
        modelBuilder.Entity<Container>().HasData(SeedData.Containers);
        modelBuilder.Entity<Shipment>().HasData(SeedData.Shipments);
        modelBuilder.Entity<ShipmentContainer>().HasData(SeedData.ShipmentContainers);
        modelBuilder.Entity<TrackingEvent>().HasData(SeedData.TrackingEvents);
    }
}
