using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Facility Maintenance entity for tracking maintenance schedules and activities
/// </summary>
public class FacilityMaintenance : BaseEntity
{
    /// <summary>
    /// Foreign key to Facility
    /// </summary>
    public Guid FacilityId { get; set; }
    
    /// <summary>
    /// Facility navigation property
    /// </summary>
    public Facility Facility { get; set; } = null!;
    
    /// <summary>
    /// Maintenance start date and time
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Maintenance end date and time
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Reason for maintenance (required)
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of maintenance work
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Maintenance status
    /// </summary>
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;
    
    /// <summary>
    /// Foreign key to User who created the maintenance record (admin)
    /// </summary>
    public Guid CreatedByUserId { get; set; }
    
    /// <summary>
    /// User who created the maintenance record navigation property
    /// </summary>
    public User Creator { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to User who completed the maintenance (admin)
    /// </summary>
    public Guid? CompletedBy { get; set; }
    
    /// <summary>
    /// User who completed the maintenance navigation property
    /// </summary>
    public User? Completer { get; set; }
    
    /// <summary>
    /// Timestamp when maintenance was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    // Domain methods
    /// <summary>
    /// Get duration in hours
    /// </summary>
    public double GetDurationHours()
    {
        return (EndDate - StartDate).TotalHours;
    }
    
    /// <summary>
    /// Check if maintenance overlaps with a given date range
    /// </summary>
    public bool OverlapsWith(DateTime start, DateTime end)
    {
        return StartDate < end && EndDate > start;
    }
    
    /// <summary>
    /// Start maintenance
    /// </summary>
    public void Start()
    {
        Status = MaintenanceStatus.InProgress;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Complete maintenance
    /// </summary>
    public void Complete(Guid completedByUserId)
    {
        Status = MaintenanceStatus.Completed;
        CompletedBy = completedByUserId;
        CompletedAt = DateTime.UtcNow;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Cancel maintenance
    /// </summary>
    public void Cancel()
    {
        Status = MaintenanceStatus.Cancelled;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Extend maintenance end date
    /// </summary>
    public void ExtendEndDate(DateTime newEndDate)
    {
        if (newEndDate <= StartDate)
            throw new ArgumentException("End date must be after start date");
            
        EndDate = newEndDate;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Check if maintenance is currently active
    /// </summary>
    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        return Status == MaintenanceStatus.InProgress || 
               (Status == MaintenanceStatus.Scheduled && StartDate <= now && EndDate >= now);
    }
    
    /// <summary>
    /// Check if maintenance is in the future
    /// </summary>
    public bool IsFuture()
    {
        return StartDate > DateTime.UtcNow;
    }
}
