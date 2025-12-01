using CleanArchitectureTemplate.Domain.Commons;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Holiday entity for managing system-wide holidays
/// </summary>
public class Holiday : BaseEntity
{
    /// <summary>
    /// Holiday name (e.g., "Tet Holiday", "National Day")
    /// </summary>
    public string HolidayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Date of the holiday
    /// </summary>
    public DateTime HolidayDate { get; set; }
    
    /// <summary>
    /// Whether this holiday recurs annually (e.g., New Year's Day)
    /// </summary>
    public bool IsRecurring { get; set; } = false;
    
    /// <summary>
    /// Description or notes about the holiday
    /// </summary>
    public string? Description { get; set; }
    
    // Domain methods
    /// <summary>
    /// Check if a given date falls on this holiday
    /// </summary>
    public bool IsHoliday(DateTime date)
    {
        if (IsRecurring)
        {
            return HolidayDate.Month == date.Month && HolidayDate.Day == date.Day;
        }
        
        return HolidayDate.Date == date.Date;
    }
}
