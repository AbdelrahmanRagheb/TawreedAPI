using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Subtotal).HasColumnType("decimal(12,2)");
        builder.Property(i => i.DeliveryFee).HasColumnType("decimal(12,2)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(12,2)");
        builder.Property(i => i.Total).HasColumnType("decimal(12,2)");
        builder.Property(i => i.PaymentMethod).HasMaxLength(10).IsRequired();
        builder.Property(i => i.PaymentStatus).HasMaxLength(20).IsRequired();
        builder.Property(i => i.ShippingAddress).HasMaxLength(500).IsRequired();
        builder.Property(i => i.ShippingLatitude).HasColumnType("decimal(9,6)");
        builder.Property(i => i.ShippingLongitude).HasColumnType("decimal(9,6)");

        builder.HasOne(i => i.GroupOrder)
            .WithMany(o => o.Invoices)
            .HasForeignKey(i => i.GroupOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Buyer)
            .WithMany(b => b.Invoices)
            .HasForeignKey(i => i.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Participant)
            .WithMany(p => p.Invoices)
            .HasForeignKey(i => i.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
