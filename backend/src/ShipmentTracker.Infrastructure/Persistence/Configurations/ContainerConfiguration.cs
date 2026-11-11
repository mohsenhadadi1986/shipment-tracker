using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipmentTracker.Domain.Entities;

namespace ShipmentTracker.Infrastructure.Persistence.Configurations;

public class ContainerConfiguration : IEntityTypeConfiguration<Container>
{
    public void Configure(EntityTypeBuilder<Container> builder)
    {
        builder.ToTable("Containers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContainerNumber)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(c => c.ContainerNumber)
            .IsUnique();

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(c => c.CurrentVessel)
            .WithMany(v => v.Containers)
            .HasForeignKey(c => c.CurrentVesselId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
