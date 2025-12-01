using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Facility entity representing bookable facilities (rooms, labs, sports facilities, etc.)
/// </summary>
public class Facility : BaseEntity
{
    /// <summary>
    /// Facility code (unique identifier, e.g., "LAB-501", "MEET-A-201")
    /// </summary>
    public string FacilityCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Facility name (e.g., "Computer Lab 501", "Meeting Room A-201")
    /// </summary>
    public string FacilityName { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to FacilityType
    /// </summary>
    public Guid TypeId { get; set; }
    
    /// <summary>
    /// FacilityType navigation property
    /// </summary>
    public FacilityType Type { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to Campus
    /// </summary>
    public Guid CampusId { get; set; }
    
    /// <summary>
    /// Campus navigation property
    /// </summary>
    public Campus Campus { get; set; } = null!;
    
    /// <summary>
    /// Building name or code
    /// </summary>
    public string? Building { get; set; }
    
    /// <summary>
    /// Floor (e.g., "1", "2", "Ground", "Basement 1")
    /// </summary>
    public string? Floor { get; set; }
    
    /// <summary>
    /// Room number
    /// </summary>
    public string? RoomNumber { get; set; }
    
    /// <summary>
    /// Maximum capacity (number of people)
    /// </summary>
    public int Capacity { get; set; }
    
    /// <summary>
    /// Detailed description of the facility
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Equipment available (JSON or comma-separated list)
    /// Examples: "Projector, Whiteboard, 30 PCs, AC"
    /// </summary>
    public string? Equipment { get; set; }
    
    /// <summary>
    /// Main image URL for the facility
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Facility status (Available, Under Maintenance, Unavailable)
    /// </summary>
    public FacilityStatus Status { get; set; } = FacilityStatus.Available;
    
    /// <summary>
    /// Whether the facility is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    /// <summary>
    /// Bookings for this facility
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    
    /// <summary>
    /// Maintenance records for this facility
    /// </summary>
    public ICollection<FacilityMaintenance> MaintenanceRecords { get; set; } = new List<FacilityMaintenance>();
    
    // Domain methods
    /// <summary>
    /// Check if capacity is sufficient for number of participants
    /// </summary>
    public bool HasSufficientCapacity(int participants)
    {
        return participants <= Capacity;
    }
    
    /// <summary>
    /// Set facility status
    /// </summary>
    public void SetStatus(FacilityStatus newStatus)
    {
        Status = newStatus;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Mark facility as available
    /// </summary>
    public void MarkAsAvailable()
    {
        Status = FacilityStatus.Available;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Mark facility as under maintenance
    /// </summary>
    public void MarkAsUnderMaintenance()
    {
        Status = FacilityStatus.UnderMaintenance;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Mark facility as unavailable
    /// </summary>
    public void MarkAsUnavailable()
    {
        Status = FacilityStatus.Unavailable;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Activate facility
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Deactivate facility (soft delete)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Get full location string
    /// </summary>
    public string GetFullLocation()
    {
        var parts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(Building))
            parts.Add($"Building {Building}");
            
        if (!string.IsNullOrWhiteSpace(Floor))
            parts.Add($"Floor {Floor}");
            
        if (!string.IsNullOrWhiteSpace(RoomNumber))
            parts.Add($"Room {RoomNumber}");
        
        return string.Join(", ", parts);
    }
}
