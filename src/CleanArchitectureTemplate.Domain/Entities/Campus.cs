using CleanArchitectureTemplate.Domain.Commons;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Campus entity representing FPT University campuses
/// </summary>
public class Campus : BaseEntity
{
    /// <summary>
    /// Campus name (e.g., "FPT HCM Campus 1", "FPT HCM Campus 2")
    /// </summary>
    public string CampusName { get; set; } = string.Empty;
    
    /// <summary>
    /// Campus code (unique identifier, e.g., "HCM-C1", "HCM-C2")
    /// </summary>
    public string CampusCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Full address of the campus
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Campus working hours start time (default: 07:00)
    /// </summary>
    public TimeSpan WorkingHoursStart { get; set; } = new TimeSpan(7, 0, 0);
    
    /// <summary>
    /// Campus working hours end time (default: 22:00)
    /// </summary>
    public TimeSpan WorkingHoursEnd { get; set; } = new TimeSpan(22, 0, 0);
    
    /// <summary>
    /// Whether the campus is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    /// <summary>
    /// Facilities in this campus
    /// </summary>
    public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    
    /// <summary>
    /// Users belonging to this campus
    /// </summary>
    public ICollection<User> Users { get; set; } = new List<User>();
    
    // Domain methods
    /// <summary>
    /// Check if a given time is within campus working hours
    /// </summary>
    public bool IsWithinWorkingHours(TimeSpan time)
    {
        return time >= WorkingHoursStart && time <= WorkingHoursEnd;
    }
    
    /// <summary>
    /// Check if a time range is within campus working hours
    /// </summary>
    public bool IsTimeRangeValid(TimeSpan startTime, TimeSpan endTime)
    {
        return IsWithinWorkingHours(startTime) && 
               IsWithinWorkingHours(endTime) && 
               startTime < endTime;
    }
    
    /// <summary>
    /// Activate campus
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Deactivate campus (temporarily close)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        this.MarkAsModified();
    }
}
