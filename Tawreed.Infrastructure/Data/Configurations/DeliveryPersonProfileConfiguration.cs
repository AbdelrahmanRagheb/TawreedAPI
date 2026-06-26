using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class DeliveryPersonProfileConfiguration : IEntityTypeConfiguration<DeliveryPersonProfile>
{
    public void Configure(EntityTypeBuilder<DeliveryPersonProfile> builder)
    {
        builder.ToTable("delivery_person_profiles");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.VehicleType).HasMaxLength(50).IsRequired();
        builder.Property(d => d.LicenseInfo).HasMaxLength(500);
        builder.Property(d => d.BaseDeliveryFee).HasColumnType("decimal(12,2)");
        builder.Property(d => d.CoverageRegionId).HasColumnName("coverage_region_id");
        builder.Property(d => d.Rating).HasColumnType("decimal(2,1)");

        builder.HasOne(d => d.User)
            .WithOne(u => u.DeliveryPersonProfile)
            .HasForeignKey<DeliveryPersonProfile>(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.CoverageRegion)
            .WithMany()
            .HasForeignKey(d => d.CoverageRegionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(d => d.UserId).IsUnique();
        builder.HasIndex(d => d.CoverageRegionId);
    }
}