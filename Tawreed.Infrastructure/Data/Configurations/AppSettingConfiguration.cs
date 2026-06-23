using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
{
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("app_settings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasIndex(x => x.Key).IsUnique();
    }
}
