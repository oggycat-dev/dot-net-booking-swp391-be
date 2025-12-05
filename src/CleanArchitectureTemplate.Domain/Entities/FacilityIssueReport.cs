using CleanArchitectureTemplate.Domain.Commons;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Facility issue report from users during their booking
/// </summary>
public class FacilityIssueReport : BaseEntity
{
    /// <summary>
    /// Report code (unique identifier)
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to Booking
    /// </summary>
    public Guid BookingId { get; set; }
    
    /// <summary>
    /// Booking navigation property
    /// </summary>
    public Booking Booking { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to User who reported the issue
    /// </summary>
    public Guid ReportedBy { get; set; }
    
    /// <summary>
    /// User who reported navigation property
    /// </summary>
    public User ReportedByUser { get; set; } = null!;
    
    /// <summary>
    /// Issue title/summary
    /// </summary>
    public string IssueTitle { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the issue
    /// </summary>
    public string IssueDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Severity level (Low, Medium, High, Critical)
    /// </summary>
    public string Severity { get; set; } = "Medium";
    
    /// <summary>
    /// Issue category (e.g., "Leak", "Equipment", "Cleanliness", "Safety", "Other")
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Images/evidence URLs (JSON array)
    /// </summary>
    public string? ImageUrls { get; set; }
    
    /// <summary>
    /// Report status (Pending, UnderReview, Resolved, RoomChanged)
    /// </summary>
    public string Status { get; set; } = "Pending";
    
    /// <summary>
    /// New facility ID if room was changed
    /// </summary>
    public Guid? NewFacilityId { get; set; }
    
    /// <summary>
    /// New facility navigation property
    /// </summary>
    public Facility? NewFacility { get; set; }
    
    /// <summary>
    /// Admin who handled the report
    /// </summary>
    public Guid? HandledBy { get; set; }
    
    /// <summary>
    /// Admin handler navigation property
    /// </summary>
    public User? HandledByUser { get; set; }
    
    /// <summary>
    /// Admin's response/resolution notes
    /// </summary>
    public string? AdminResponse { get; set; }
    
    /// <summary>
    /// When the admin handled the report
    /// </summary>
    public DateTime? HandledAt { get; set; }
    
    /// <summary>
    /// When the report was resolved
    /// </summary>
    public DateTime? ResolvedAt { get; set; }
}
