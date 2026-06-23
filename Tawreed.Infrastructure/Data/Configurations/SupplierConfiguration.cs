using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.CompanyName).HasMaxLength(200).IsRequired();

        builder.HasIndex(s => s.UserId).IsUnique();
        builder.Property(s => s.TaxId).HasMaxLength(50);
        builder.Property(s => s.RatingAvg).HasColumnType("decimal(3,2)");
        builder.Property(s => s.Address).HasMaxLength(500);

        builder.HasOne(s => s.User)
            .WithOne(u => u.Supplier)
            .HasForeignKey<Supplier>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Region)
            .WithMany(r => r.Suppliers)
            .HasForeignKey(s => s.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Approver)
            .WithMany()
            .HasForeignKey(s => s.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
