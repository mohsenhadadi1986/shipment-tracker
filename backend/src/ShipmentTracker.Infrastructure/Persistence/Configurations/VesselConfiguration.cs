using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Domain.Entities;

namespace ShipmentTracker.Infrastructure.Persistence.Configurations;

public class VesselConfiguration : IEntityTypeConfiguration<Vessel>
{
    public void Configure(EntityTypeBuilder<Vessel> builder)
    {
        builder.ToTable("Vessels");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.ImoNumber)
            .IsRequired()
            .HasMaxLength(7)
            .IsFixedLength();

        builder.HasIndex(v => v.ImoNumber)
            .IsUnique();

        builder.Property(v => v.FlagCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.CapacityTeu)
            .IsRequired();
    }
}
