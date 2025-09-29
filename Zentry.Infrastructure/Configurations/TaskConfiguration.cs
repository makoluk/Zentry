using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentry.Domain.Entities;

namespace Zentry.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for TaskItem
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.HasKey(w => w.Id);

        // Audit fields first
        builder.Property(w => w.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        builder.Property(w => w.UpdatedAtUtc)
            .IsRequired(false); // NULL başlangıç, güncellemede set edilecek

        // Business fields
        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .HasMaxLength(1000);

        builder.Property(w => w.IsDone)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(w => w.CategoryId)
            .IsRequired();

        // Foreign key relationship
        builder.HasOne(w => w.Category)
            .WithMany(c => c.Tasks)
            .HasForeignKey(w => w.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(w => w.IsDone);
        builder.HasIndex(w => w.UpdatedAtUtc);
        builder.HasIndex(w => w.CategoryId);

        // Table name
        builder.ToTable("Tasks");
    }
}
