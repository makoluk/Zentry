using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentry.Domain.Entities;

namespace Zentry.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for Category
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.HasKey(c => c.Id);

        // Audit fields first
        builder.Property(c => c.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired(false); // NULL başlangıç, güncellemede set edilecek

        // Business fields
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Color)
            .HasMaxLength(7); // #RRGGBB format

        builder.Property(c => c.Icon)
            .HasMaxLength(50);

        builder.Property(c => c.SortOrder)
            .HasDefaultValue(0);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.SortOrder);
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => c.CreatedAtUtc);
        builder.HasIndex(c => c.UpdatedAtUtc);

        // Table name
        builder.ToTable("Categories");
    }
}
