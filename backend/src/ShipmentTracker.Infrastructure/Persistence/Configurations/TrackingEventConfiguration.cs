using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Domain.Entities;

namespace ShipmentTracker.Infrastructure.Persistence.Configurations;

public class TrackingEventConfiguration : IEntityTypeConfiguration<TrackingEvent>
{
    public void Configure(EntityTypeBuilder<TrackingEvent> builder)
    {
        builder.ToTable("TrackingEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.OccurredAt)
            .IsRequired();

        builder.HasOne(e => e.Shipment)
            .WithMany(s => s.TrackingEvents)
            .HasForeignKey(e => e.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Container)
            .WithMany(c => c.TrackingEvents)
            .HasForeignKey(e => e.ContainerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Port)
            .WithMany(p => p.TrackingEvents)
            .HasForeignKey(e => e.PortId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.ShipmentId);
        builder.HasIndex(e => e.OccurredAt);
    }
}
