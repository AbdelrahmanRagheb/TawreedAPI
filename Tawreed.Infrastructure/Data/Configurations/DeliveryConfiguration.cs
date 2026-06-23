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
        builder.Property(d => d.ShippingAddress).HasMaxLength(500).IsRequired();
        builder.Property(d => d.ShippingLatitude).HasColumnType("decimal(9,6)");
        builder.Property(d => d.ShippingLongitude).HasColumnType("decimal(9,6)");

        builder.HasOne(d => d.Invoice)
            .WithOne(i => i.Delivery)
            .HasForeignKey<Delivery>(d => d.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

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

        builder.HasIndex(d => d.InvoiceId).IsUnique();
    }
}
