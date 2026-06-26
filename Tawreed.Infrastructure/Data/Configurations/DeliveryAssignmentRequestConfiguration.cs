using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class DeliveryAssignmentRequestConfiguration : IEntityTypeConfiguration<DeliveryAssignmentRequest>
{
    public void Configure(EntityTypeBuilder<DeliveryAssignmentRequest> builder)
    {
        builder.ToTable("delivery_assignment_requests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status).HasMaxLength(20).IsRequired();
        builder.Property(r => r.DeclineReason).HasMaxLength(500);
        builder.Property(r => r.ProposedFee).HasColumnType("decimal(12,2)");

        builder.HasOne(r => r.Order)
            .WithMany()
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.DeliveryPerson)
            .WithMany()
            .HasForeignKey(r => r.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Supplier)
            .WithMany()
            .HasForeignKey(r => r.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.OrderId, r.DeliveryPersonId });
        builder.HasIndex(r => r.Status);
    }
}
