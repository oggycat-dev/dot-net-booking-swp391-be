namespace CleanArchitectureTemplate.Application.Common.DTOs.Notifications;

/// <summary>
/// Request to register FCM token for push notifications
/// </summary>
public class RegisterFcmTokenRequest
{
    /// <summary>
    /// FCM device token from Firebase client SDK
    /// </summary>
    public string FcmToken { get; set; } = string.Empty;
}

/// <summary>
/// Notification payload for booking events
/// </summary>
public class BookingNotificationPayload
{
    /// <summary>
    /// Type of notification (e.g., "new_booking", "booking_approved", "booking_rejected")
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Booking ID
    /// </summary>
    public Guid BookingId { get; set; }

    /// <summary>
    /// User who created the booking
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Facility name
    /// </summary>
    public string FacilityName { get; set; } = string.Empty;

    /// <summary>
    /// Booking date
    /// </summary>
    public string BookingDate { get; set; } = string.Empty;

    /// <summary>
    /// Booking start time
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// Booking end time
    /// </summary>
    public string EndTime { get; set; } = string.Empty;
}
