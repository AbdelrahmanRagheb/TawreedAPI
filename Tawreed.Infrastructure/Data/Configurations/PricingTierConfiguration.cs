using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class PricingTierConfiguration : IEntityTypeConfiguration<PricingTier>
{
    public void Configure(EntityTypeBuilder<PricingTier> builder)
    {
        builder.ToTable("pricing_tiers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UnitPrice).HasColumnType("decimal(12,2)");

        builder.HasOne(t => t.SupplierProduct)
            .WithMany(sp => sp.PricingTiers)
            .HasForeignKey(t => t.SupplierProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
