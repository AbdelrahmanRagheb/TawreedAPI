using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class GroupOrderParticipantConfiguration : IEntityTypeConfiguration<GroupOrderParticipant>
{
    public void Configure(EntityTypeBuilder<GroupOrderParticipant> builder)
    {
        builder.ToTable("group_order_participants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status).HasMaxLength(20).IsRequired();

        builder.HasOne(p => p.GroupOrder)
            .WithMany(o => o.Participants)
            .HasForeignKey(p => p.GroupOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Buyer)
            .WithMany(b => b.Participants)
            .HasForeignKey(p => p.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.GroupOrderId, p.BuyerId }).IsUnique();
    }
}
