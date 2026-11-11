using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Domain.Entities;

namespace ShipmentTracker.Infrastructure.Persistence.Configurations;

public class ShipmentContainerConfiguration : IEntityTypeConfiguration<ShipmentContainer>
{
    public void Configure(EntityTypeBuilder<ShipmentContainer> builder)
    {
        builder.ToTable("ShipmentContainers");

        // Composite PK: one container appears at most once per shipment
        builder.HasKey(sc => new { sc.ShipmentId, sc.ContainerId });

        builder.Property(sc => sc.SealNumber)
            .HasMaxLength(50);

        builder.HasOne(sc => sc.Shipment)
            .WithMany(s => s.ShipmentContainers)
            .HasForeignKey(sc => sc.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.Container)
            .WithMany(c => c.ShipmentContainers)
            .HasForeignKey(sc => sc.ContainerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
