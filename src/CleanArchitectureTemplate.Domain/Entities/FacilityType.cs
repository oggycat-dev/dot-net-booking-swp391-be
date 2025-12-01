using CleanArchitectureTemplate.Domain.Commons;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Facility Type entity (e.g., Meeting Room, Lab, Sports Facility, Study Room)
/// </summary>
public class FacilityType : BaseEntity
{
    /// <summary>
    /// Type name (e.g., "Computer Lab", "Meeting Room", "Sports Facility")
    /// </summary>
    public string TypeName { get; set; } = string.Empty;
    
    /// <summary>
    /// Type code (unique identifier, e.g., "LAB", "MEET", "SPORT")
    /// </summary>
    public string TypeCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the facility type
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Default booking duration in minutes (e.g., 60, 120)
    /// </summary>
    public int DefaultDuration { get; set; } = 60;
    
    /// <summary>
    /// Whether bookings for this type require admin approval
    /// </summary>
    public bool RequiresApproval { get; set; } = true;
    
    /// <summary>
    /// Icon URL for display purposes
    /// </summary>
    public string? IconUrl { get; set; }
    
    /// <summary>
    /// Whether this facility type is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    /// <summary>
    /// Facilities of this type
    /// </summary>
    public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    
    // Domain methods
    /// <summary>
    /// Activate facility type
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Deactivate facility type
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        this.MarkAsModified();
    }
}
