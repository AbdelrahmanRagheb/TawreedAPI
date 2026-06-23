using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class ParticipantItemConfiguration : IEntityTypeConfiguration<ParticipantItem>
{
    public void Configure(EntityTypeBuilder<ParticipantItem> builder)
    {
        builder.ToTable("participant_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.UnitPriceAtJoin).HasColumnType("decimal(12,2)");
        builder.Property(i => i.FinalUnitPrice).HasColumnType("decimal(12,2)");
        builder.Property(i => i.LineTotal).HasColumnType("decimal(12,2)");

        builder.HasOne(i => i.Participant)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.GroupOrderItem)
            .WithMany(goi => goi.ParticipantItems)
            .HasForeignKey(i => i.GroupOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
