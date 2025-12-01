using CleanArchitectureTemplate.Domain.Commons;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Booking History entity for audit trail of all booking status changes
/// </summary>
public class BookingHistory : BaseEntity
{
    /// <summary>
    /// Foreign key to Booking
    /// </summary>
    public Guid BookingId { get; set; }
    
    /// <summary>
    /// Booking navigation property
    /// </summary>
    public Booking Booking { get; set; } = null!;
    
    /// <summary>
    /// Previous status (null if this is the first status)
    /// </summary>
    public string? StatusFrom { get; set; }
    
    /// <summary>
    /// New status
    /// </summary>
    public string StatusTo { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to User who made the change
    /// </summary>
    public Guid ChangedByUserId { get; set; }
    
    /// <summary>
    /// User who made the change navigation property
    /// </summary>
    public User ChangedBy { get; set; } = null!;
    
    /// <summary>
    /// Timestamp when change was made
    /// </summary>
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Additional notes about the change
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// IP address of the user who made the change
    /// </summary>
    public string? IpAddress { get; set; }
    
    // Domain methods
    /// <summary>
    /// Create history record for status change
    /// </summary>
    public static BookingHistory CreateStatusChange(
        Guid bookingId, 
        string? statusFrom, 
        string statusTo, 
        Guid changedByUserId,
        string? note = null,
        string? ipAddress = null)
    {
        return new BookingHistory
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            StatusFrom = statusFrom,
            StatusTo = statusTo,
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTime.UtcNow,
            Note = note,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };
    }
}
