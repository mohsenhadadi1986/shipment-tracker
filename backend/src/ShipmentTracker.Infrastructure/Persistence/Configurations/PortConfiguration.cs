using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Domain.Entities;

namespace ShipmentTracker.Infrastructure.Persistence.Configurations;

public class PortConfiguration : IEntityTypeConfiguration<Port>
{
    public void Configure(EntityTypeBuilder<Port> builder)
    {
        builder.ToTable("Ports");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.UnLocode)
            .IsRequired()
            .HasMaxLength(5)
            .IsFixedLength();

        builder.HasIndex(p => p.UnLocode)
            .IsUnique();

        builder.Property(p => p.Latitude)
            .HasPrecision(9, 6);

        builder.Property(p => p.Longitude)
            .HasPrecision(9, 6);
    }
}
