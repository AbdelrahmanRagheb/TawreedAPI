using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("product_images");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl).HasMaxLength(500).IsRequired();

        builder.HasOne(i => i.SupplierProduct)
            .WithMany(sp => sp.Images)
            .HasForeignKey(i => i.SupplierProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
