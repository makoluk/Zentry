using Zentry.Domain.Common;

namespace Zentry.Domain.Entities;

/// <summary>
/// Daily habit entry for tracking habit completion
/// </summary>
public class HabitEntry : BaseEntity
{
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; } // Hangi gün için kayıt
    public bool IsCompleted { get; set; } // Boolean habit'ler için
    public int? Value { get; set; } // Numeric habit'ler için değer
    public string? Notes { get; set; } // Opsiyonel notlar
    
    // Navigation property
    public Habit Habit { get; set; } = null!;
}
