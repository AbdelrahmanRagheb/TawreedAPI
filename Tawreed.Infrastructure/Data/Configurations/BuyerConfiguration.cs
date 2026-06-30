using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.ToTable("buyers");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BusinessName).HasMaxLength(200).IsRequired();

        builder.HasIndex(b => b.UserId).IsUnique();
        builder.Property(b => b.BusinessType).HasMaxLength(20).IsRequired();
        builder.Property(b => b.TaxId).HasMaxLength(50);
        builder.Property(b => b.CommercialRegistrationNo).HasMaxLength(100);
        builder.HasOne(b => b.User)
            .WithOne(u => u.Buyer)
            .HasForeignKey<Buyer>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Region)
            .WithMany(r => r.Buyers)
            .HasForeignKey(b => b.RegionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
