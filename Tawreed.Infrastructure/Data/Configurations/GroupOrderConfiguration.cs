using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class GroupOrderConfiguration : IEntityTypeConfiguration<GroupOrder>
{
    public void Configure(EntityTypeBuilder<GroupOrder> builder)
    {
        builder.ToTable("group_orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title).HasMaxLength(200).IsRequired();
        builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Notes).HasColumnType("nvarchar(max)");
        builder.Property(o => o.Visibility).HasMaxLength(10);
        builder.Property(o => o.Status).HasMaxLength(20).IsRequired();
        builder.Property(o => o.DeliveryPreference).HasMaxLength(20);
        builder.Property(o => o.DeliveryApprovalStatus).HasMaxLength(20);

        builder.HasOne(o => o.Creator)
            .WithMany(b => b.CreatedGroupOrders)
            .HasForeignKey(o => o.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Supplier)
            .WithMany(s => s.GroupOrders)
            .HasForeignKey(o => o.SupplierId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(o => o.Region)
            .WithMany(r => r.GroupOrders)
            .HasForeignKey(o => o.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.AssignedDeliveryPerson)
            .WithMany()
            .HasForeignKey(o => o.AssignedDeliveryPersonId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
