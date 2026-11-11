using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Domain.Entities;

namespace ShipmentTracker.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.ReferenceNumber)
            .IsUnique();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasOne(s => s.OriginPort)
            .WithMany(p => p.OriginShipments)
            .HasForeignKey(s => s.OriginPortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.DestinationPort)
            .WithMany(p => p.DestinationShipments)
            .HasForeignKey(s => s.DestinationPortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Vessel)
            .WithMany(v => v.Shipments)
            .HasForeignKey(s => s.VesselId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.OriginPortId);
        builder.HasIndex(s => s.DestinationPortId);
        builder.HasIndex(s => s.Status);
    }
}
