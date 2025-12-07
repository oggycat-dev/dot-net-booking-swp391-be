using CleanArchitectureTemplate.Domain.Commons;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Notification entity for storing system notifications
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// User who receives this notification
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Notification title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Notification body/message
    /// </summary>
    public string Body { get; set; } = string.Empty;
    
    /// <summary>
    /// Notification type (e.g., "booking", "registration", "campus_change", "facility_issue")
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// Related entity ID (e.g., BookingId, UserId, CampusChangeRequestId, FacilityIssueReportId)
    /// </summary>
    public Guid? RelatedEntityId { get; set; }
    
    /// <summary>
    /// Whether notification has been read
    /// </summary>
    public bool IsRead { get; set; }
    
    /// <summary>
    /// When notification was read
    /// </summary>
    public DateTime? ReadAt { get; set; }
    
    /// <summary>
    /// Additional data as JSON
    /// </summary>
    public string? Data { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
}
