using CleanArchitectureTemplate.Application.Common.DTOs.Notifications;

namespace CleanArchitectureTemplate.Application.Common.Interfaces;

/// <summary>
/// Service for sending Firebase Cloud Messaging notifications
/// </summary>
public interface IFirebaseNotificationService
{
    /// <summary>
    /// Send notification to all admin users
    /// </summary>
    /// <param name="title">Notification title</param>
    /// <param name="body">Notification body</param>
    /// <param name="data">Additional data payload</param>
    /// <returns>Number of notifications successfully sent</returns>
    Task<int> SendToAllAdminsAsync(string title, string body, Dictionary<string, string>? data = null);

    /// <summary>
    /// Send notification to specific user by FCM token
    /// </summary>
    /// <param name="fcmToken">Firebase Cloud Messaging token</param>
    /// <param name="title">Notification title</param>
    /// <param name="body">Notification body</param>
    /// <param name="data">Additional data payload</param>
    /// <returns>True if sent successfully</returns>
    Task<bool> SendToTokenAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null);

    /// <summary>
    /// Send notification to specific user by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="title">Notification title</param>
    /// <param name="body">Notification body</param>
    /// <param name="data">Additional data payload</param>
    /// <returns>True if sent successfully</returns>
    Task<bool> SendToUserAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null);

    /// <summary>
    /// Send booking notification to all admins
    /// </summary>
    /// <param name="payload">Booking notification payload</param>
    /// <returns>Number of notifications successfully sent</returns>
    Task<int> SendBookingNotificationToAdminsAsync(BookingNotificationPayload payload);
}
