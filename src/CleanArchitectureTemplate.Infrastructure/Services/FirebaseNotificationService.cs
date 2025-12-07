using CleanArchitectureTemplate.Application.Common.DTOs.Notifications;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FcmNotification = FirebaseAdmin.Messaging.Notification;

namespace CleanArchitectureTemplate.Infrastructure.Services;

/// <summary>
/// Firebase Cloud Messaging notification service
/// </summary>
public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FirebaseNotificationService> _logger;
    private readonly IConfiguration _configuration;
    private static bool _isInitialized = false;
    private static readonly object _lock = new object();

    public FirebaseNotificationService(
        IUnitOfWork unitOfWork,
        ILogger<FirebaseNotificationService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _configuration = configuration;
        
        InitializeFirebase();
    }

    /// <summary>
    /// Initialize Firebase Admin SDK
    /// </summary>
    private void InitializeFirebase()
    {
        if (_isInitialized) return;

        lock (_lock)
        {
            if (_isInitialized) return;

            try
            {
                var credentialPath = _configuration["Firebase:CredentialPath"];
                
                if (string.IsNullOrEmpty(credentialPath))
                {
                    _logger.LogWarning("Firebase credential path not configured. Push notifications will be disabled.");
                    return;
                }

                if (!File.Exists(credentialPath))
                {
                    _logger.LogWarning("Firebase credential file not found at {Path}. Push notifications will be disabled.", credentialPath);
                    return;
                }

                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });

                _isInitialized = true;
                _logger.LogInformation("Firebase Admin SDK initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase Admin SDK");
            }
        }
    }

    /// <summary>
    /// Send notification to all admin users
    /// </summary>
    public async Task<int> SendToAllAdminsAsync(string title, string body, Dictionary<string, string>? data = null)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notifications.");
            return 0;
        }

        try
        {
            // Get all admins with FCM tokens
            var adminUsers = await _unitOfWork.Users
                .GetQueryable()
                .Where(u => u.Role == UserRole.Admin && !string.IsNullOrEmpty(u.FcmToken))
                .ToListAsync();

            if (!adminUsers.Any())
            {
                _logger.LogInformation("No admin users with FCM tokens found");
                return 0;
            }

            _logger.LogInformation("Sending notification to {Count} admin users", adminUsers.Count);

            // Save notifications to database for all admins
            foreach (var admin in adminUsers)
            {
                var notification = new Domain.Entities.Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = admin.Id,
                    Title = title,
                    Body = body,
                    Type = data?.GetValueOrDefault("type") ?? "general",
                    RelatedEntityId = data?.ContainsKey("relatedId") == true && Guid.TryParse(data["relatedId"], out var relatedId) 
                        ? relatedId 
                        : null,
                    Data = data != null ? JsonSerializer.Serialize(data) : null,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Notifications.AddAsync(notification);
            }
            await _unitOfWork.SaveChangesAsync();

            // Send FCM push notifications
            var fcmTokens = adminUsers.Select(u => u.FcmToken!).ToList();

            var message = new MulticastMessage
            {
                Tokens = fcmTokens,
                Notification = new FcmNotification
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default",
                        ChannelId = "booking_notifications"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        Badge = 1
                    }
                }
            };

            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            
            _logger.LogInformation("Sent {SuccessCount} notifications successfully, {FailureCount} failed", 
                response.SuccessCount, response.FailureCount);

            // Log failures
            if (response.FailureCount > 0)
            {
                for (int i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        _logger.LogWarning("Failed to send to token {Token}: {Error}", 
                            fcmTokens[i], response.Responses[i].Exception?.Message);
                    }
                }
            }

            return response.SuccessCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to all admins");
            return 0;
        }
    }

    /// <summary>
    /// Send notification to specific token
    /// </summary>
    public async Task<bool> SendToTokenAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notifications.");
            return false;
        }

        try
        {
            var message = new Message
            {
                Token = fcmToken,
                Notification = new FcmNotification
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default",
                        ChannelId = "booking_notifications"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default",
                        Badge = 1
                    }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Notification sent successfully to token");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to token {Token}", fcmToken);
            return false;
        }
    }

    /// <summary>
    /// Send notification to specific user
    /// </summary>
    public async Task<bool> SendToUserAsync(Guid userId, string title, string body, Dictionary<string, string>? data = null)
    {
        if (!_isInitialized)
        {
            _logger.LogWarning("Firebase not initialized. Cannot send notifications.");
            return false;
        }

        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return false;
            }

            // Save notification to database
            var notification = new Domain.Entities.Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Body = body,
                Type = data?.GetValueOrDefault("type") ?? "general",
                RelatedEntityId = data?.ContainsKey("relatedId") == true && Guid.TryParse(data["relatedId"], out var relatedId) 
                    ? relatedId 
                    : null,
                Data = data != null ? JsonSerializer.Serialize(data) : null,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            // Send FCM push notification if user has token
            if (!string.IsNullOrEmpty(user.FcmToken))
            {
                return await SendToTokenAsync(user.FcmToken, title, body, data);
            }

            return true; // Notification saved to DB even if no FCM token
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Send booking notification to all admins
    /// </summary>
    public async Task<int> SendBookingNotificationToAdminsAsync(BookingNotificationPayload payload)
    {
        var title = "New Booking Request";
        var body = $"{payload.UserName} requested to book {payload.FacilityName} on {payload.BookingDate} ({payload.StartTime} - {payload.EndTime})";

        var data = new Dictionary<string, string>
        {
            { "type", payload.Type },
            { "bookingId", payload.BookingId.ToString() },
            { "userName", payload.UserName },
            { "facilityName", payload.FacilityName },
            { "bookingDate", payload.BookingDate },
            { "startTime", payload.StartTime },
            { "endTime", payload.EndTime }
        };

        return await SendToAllAdminsAsync(title, body, data);
    }
}
