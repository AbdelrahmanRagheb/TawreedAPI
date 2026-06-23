using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class SupplierApprovalLogConfiguration : IEntityTypeConfiguration<SupplierApprovalLog>
{
    public void Configure(EntityTypeBuilder<SupplierApprovalLog> builder)
    {
        builder.ToTable("supplier_approval_logs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Action).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Reason).HasMaxLength(500);

        builder.HasOne(l => l.Supplier)
            .WithMany(s => s.ApprovalLogs)
            .HasForeignKey(l => l.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Actor)
            .WithMany()
            .HasForeignKey(l => l.ActorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
