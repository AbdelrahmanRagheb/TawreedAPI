using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("deliveries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Status).HasMaxLength(20).IsRequired();
        builder.Property(d => d.TrackingNotes).HasColumnType("nvarchar(max)");
        builder.Property(d => d.ShippingRegion).HasMaxLength(500).IsRequired();

        builder.Property(d => d.DeliveryFee).HasColumnType("decimal(12,2)");
        builder.Property(d => d.DeliveryType).HasMaxLength(20);

        builder.HasOne(d => d.GroupOrder)
            .WithMany(o => o.Deliveries)
            .HasForeignKey(d => d.GroupOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Supplier)
            .WithMany(s => s.Deliveries)
            .HasForeignKey(d => d.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.DeliveryPerson)
            .WithMany()
            .HasForeignKey(d => d.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
