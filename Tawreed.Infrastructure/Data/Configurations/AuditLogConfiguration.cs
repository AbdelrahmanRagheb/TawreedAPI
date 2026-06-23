using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Action).HasMaxLength(100).IsRequired();
        builder.Property(l => l.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Metadata).HasColumnType("nvarchar(max)");

        builder.HasOne(l => l.Actor)
            .WithMany()
            .HasForeignKey(l => l.ActorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
