using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.ToTable("units");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.NameAr).HasMaxLength(50).IsRequired();
        builder.Property(u => u.NameEn).HasMaxLength(50).IsRequired();
        builder.Property(u => u.Symbol).HasMaxLength(10).IsRequired();
    }
}
