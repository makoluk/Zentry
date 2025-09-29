using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentry.Domain.Entities;

namespace Zentry.Infrastructure.Configurations;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder.ToTable("Habits");
        
        builder.HasKey(h => h.Id);
        
        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(h => h.Description)
            .HasMaxLength(500);
            
        builder.Property(h => h.Color)
            .IsRequired()
            .HasMaxLength(7); // #RRGGBB format
            
        builder.Property(h => h.Icon)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(h => h.Type)
            .IsRequired()
            .HasConversion<int>();
            
        builder.Property(h => h.Unit)
            .HasMaxLength(50);
            
        builder.Property(h => h.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(h => h.SortOrder)
            .IsRequired();
            
        // Timestamp configuration
        builder.Property(h => h.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(h => h.UpdatedAtUtc)
            .IsRequired(false);
        
        // Indexes
        builder.HasIndex(h => h.IsActive);
        builder.HasIndex(h => h.SortOrder);
        builder.HasIndex(h => new { h.IsActive, h.SortOrder });
        
        // Relationships
        builder.HasMany(h => h.Entries)
            .WithOne(e => e.Habit)
            .HasForeignKey(e => e.HabitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
