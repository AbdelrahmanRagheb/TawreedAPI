using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("regions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.NameAr).HasMaxLength(200).IsRequired();
        builder.Property(r => r.NameEn).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Type)
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(r => r.Parent)
            .WithMany(r => r.Children)
            .HasForeignKey(r => r.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
