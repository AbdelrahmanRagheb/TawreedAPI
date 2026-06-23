using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class SupplierCategoryConfiguration : IEntityTypeConfiguration<SupplierCategory>
{
    public void Configure(EntityTypeBuilder<SupplierCategory> builder)
    {
        builder.ToTable("supplier_categories");

        builder.HasKey(sc => new { sc.SupplierId, sc.CategoryId });

        builder.HasOne(sc => sc.Supplier)
            .WithMany(s => s.SupplierCategories)
            .HasForeignKey(sc => sc.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.Category)
            .WithMany(c => c.SupplierCategories)
            .HasForeignKey(sc => sc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
