using Zentry.Domain.Common;

namespace Zentry.Domain.Entities;

/// <summary>
/// Habit entity for tracking daily habits
/// </summary>
public class Habit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#3B82F6";
    public string Icon { get; set; } = "target";
    public HabitType Type { get; set; } = HabitType.Boolean;
    public string? Unit { get; set; } // "bardak", "sayfa", "dakika" etc.
    public int? TargetValue { get; set; } // Hedef değer (sayısal habit'ler için)
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    
    // Navigation property
    public ICollection<HabitEntry> Entries { get; set; } = new List<HabitEntry>();
}

/// <summary>
/// Habit types
/// </summary>
public enum HabitType
{
    Boolean = 0,    // Sadece tik/tick (yapıldı/yapılmadı)
    Numeric = 1     // Sayısal değer (kaç bardak su, kaç sayfa kitap etc.)
}
