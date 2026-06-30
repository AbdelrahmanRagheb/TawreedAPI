using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class GroupOrderItemConfiguration : IEntityTypeConfiguration<GroupOrderItem>
{
    public void Configure(EntityTypeBuilder<GroupOrderItem> builder)
    {
        builder.ToTable("group_order_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.UnitPrice).HasColumnType("decimal(12,2)");
        builder.Property(i => i.ItemStatus).HasMaxLength(20).HasDefaultValue("Unassigned");

        builder.HasOne(i => i.GroupOrder)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.GroupOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.GroupOrderItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.SupplierProduct)
            .WithMany(sp => sp.GroupOrderItems)
            .HasForeignKey(i => i.SupplierProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(i => i.Supplier)
            .WithMany(s => s.GroupOrderItems)
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
