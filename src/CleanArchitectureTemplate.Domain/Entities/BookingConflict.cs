using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Booking Conflict entity for detecting and resolving booking conflicts
/// </summary>
public class BookingConflict : BaseEntity
{
    /// <summary>
    /// Foreign key to first Booking in conflict
    /// </summary>
    public Guid BookingId1 { get; set; }
    
    /// <summary>
    /// First booking navigation property
    /// </summary>
    public Booking Booking1 { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to second Booking in conflict
    /// </summary>
    public Guid BookingId2 { get; set; }
    
    /// <summary>
    /// Second booking navigation property
    /// </summary>
    public Booking Booking2 { get; set; } = null!;
    
    /// <summary>
    /// Type of conflict
    /// </summary>
    public ConflictType ConflictType { get; set; }
    
    /// <summary>
    /// Timestamp when conflict was detected
    /// </summary>
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Timestamp when conflict was resolved
    /// </summary>
    public DateTime? ResolvedAt { get; set; }
    
    /// <summary>
    /// Method used to resolve the conflict
    /// </summary>
    public ResolutionMethod? ResolutionMethod { get; set; }
    
    /// <summary>
    /// Foreign key to User who resolved the conflict (admin)
    /// </summary>
    public Guid? ResolvedBy { get; set; }
    
    /// <summary>
    /// User who resolved the conflict navigation property
    /// </summary>
    public User? Resolver { get; set; }
    
    /// <summary>
    /// Notes about the resolution
    /// </summary>
    public string? ResolutionNote { get; set; }
    
    // Domain methods
    /// <summary>
    /// Check if conflict is resolved
    /// </summary>
    public bool IsResolved()
    {
        return ResolvedAt.HasValue;
    }
    
    /// <summary>
    /// Resolve conflict
    /// </summary>
    public void Resolve(ResolutionMethod method, Guid resolvedByUserId, string? note = null)
    {
        ResolutionMethod = method;
        ResolvedBy = resolvedByUserId;
        ResolvedAt = DateTime.UtcNow;
        ResolutionNote = note;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Get conflict duration in days since detection
    /// </summary>
    public int GetDaysSinceDetection()
    {
        return (DateTime.UtcNow - DetectedAt).Days;
    }
    
    /// <summary>
    /// Reopen conflict (mark as unresolved)
    /// </summary>
    public void Reopen()
    {
        ResolvedAt = null;
        ResolutionMethod = null;
        ResolvedBy = null;
        ResolutionNote = null;
        this.MarkAsModified();
    }
}
