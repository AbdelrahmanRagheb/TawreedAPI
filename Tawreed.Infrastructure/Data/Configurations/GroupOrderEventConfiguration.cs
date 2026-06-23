using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class GroupOrderEventConfiguration : IEntityTypeConfiguration<GroupOrderEvent>
{
    public void Configure(EntityTypeBuilder<GroupOrderEvent> builder)
    {
        builder.ToTable("group_order_events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.NotesAr).HasMaxLength(500);
        builder.Property(e => e.NotesEn).HasMaxLength(500);

        builder.HasOne(e => e.GroupOrder)
            .WithMany(o => o.Events)
            .HasForeignKey(e => e.GroupOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Creator)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
