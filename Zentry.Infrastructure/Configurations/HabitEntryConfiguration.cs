using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zentry.Domain.Entities;

namespace Zentry.Infrastructure.Configurations;

public class HabitEntryConfiguration : IEntityTypeConfiguration<HabitEntry>
{
    public void Configure(EntityTypeBuilder<HabitEntry> builder)
    {
        builder.ToTable("HabitEntries");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.HabitId)
            .IsRequired();
            
        builder.Property(e => e.Date)
            .IsRequired()
            .HasConversion<string>(); // SQLite için DateOnly string olarak saklanır
            
        builder.Property(e => e.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(e => e.Value)
            .IsRequired(false);
            
        builder.Property(e => e.Notes)
            .HasMaxLength(500);
            
        // Timestamp configuration
        builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");
            
        builder.Property(e => e.UpdatedAtUtc)
            .IsRequired(false);
        
        // Indexes
        builder.HasIndex(e => e.HabitId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => new { e.HabitId, e.Date }).IsUnique(); // Bir habit için günde sadece bir entry
        
        // Relationships
        builder.HasOne(e => e.Habit)
            .WithMany(h => h.Entries)
            .HasForeignKey(e => e.HabitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
