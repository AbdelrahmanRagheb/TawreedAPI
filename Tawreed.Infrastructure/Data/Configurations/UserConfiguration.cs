using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.Phone).HasMaxLength(20).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
        builder.Property(u => u.Role).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Status).HasMaxLength(20).IsRequired();
        builder.Property(u => u.PreferredLang).HasMaxLength(2).IsRequired();
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasOne(u => u.DeliveryPersonProfile)
            .WithOne(d => d.User)
            .HasForeignKey<DeliveryPersonProfile>(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
