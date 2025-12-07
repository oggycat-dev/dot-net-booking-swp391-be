using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Notifications;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Notification controller for Firebase Cloud Messaging and notification management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<NotificationsController> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Register or update FCM token for current user (Admin only)
    /// </summary>
    /// <param name="request">FCM token from Firebase client SDK</param>
    /// <returns>Success message</returns>
    [HttpPost("register-token")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> RegisterFcmToken([FromBody] RegisterFcmTokenRequest request)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        if (string.IsNullOrWhiteSpace(request.FcmToken))
        {
            return BadRequest(ApiResponse<object>.BadRequest("FCM token is required"));
        }

        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.BadRequest("User not found"));
            }

            user.FcmToken = request.FcmToken;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("FCM token registered for user {UserId}", userId);
            return Ok(ApiResponse<object>.Ok(null, "FCM token registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering FCM token for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<object>.BadRequest("Failed to register FCM token"));
        }
    }

    /// <summary>
    /// Remove FCM token for current user (on logout or device unregister)
    /// </summary>
    /// <returns>Success message</returns>
    [HttpDelete("unregister-token")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> UnregisterFcmToken()
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.BadRequest("User not found"));
            }

            user.FcmToken = null;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("FCM token unregistered for user {UserId}", userId);
            return Ok(ApiResponse<object>.Ok(null, "FCM token unregistered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering FCM token for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<object>.BadRequest("Failed to unregister FCM token"));
        }
    }

    /// <summary>
    /// Get my notifications (Admin/Lecturer/Student)
    /// </summary>
    [HttpGet("my-notifications")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<NotificationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NotificationSummaryDto>>> GetMyNotifications(
        [FromQuery] bool? isRead = null,
        [FromQuery] int? limit = 50)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        try
        {
            // Get notifications from DB
            var notifications = await _unitOfWork.Notifications
                .GetUserNotificationsAsync(userId, isRead, limit);

            // Get unread count
            var unreadCount = await _unitOfWork.Notifications.GetUnreadCountAsync(userId);

            var notificationDtos = notifications.Select(n => new NotificationDto(
                n.Id,
                n.Title,
                n.Body,
                n.Type,
                n.RelatedEntityId,
                n.IsRead,
                n.ReadAt,
                n.CreatedAt,
                n.Data
            )).ToList();

            var result = new NotificationSummaryDto(
                notifications.Count,
                unreadCount,
                notificationDtos
            );

            return Ok(new ApiResponse<NotificationSummaryDto>
            {
                Success = true,
                Message = "Notifications retrieved successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<NotificationSummaryDto>.BadRequest("Failed to get notifications"));
        }
    }

    /// <summary>
    /// Mark notification as read (Admin/Lecturer/Student)
    /// </summary>
    [HttpPost("{notificationId}/mark-read")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(Guid notificationId)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        try
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return NotFound(ApiResponse<object>.BadRequest("Notification not found"));
            }

            // Verify ownership
            if (notification.UserId != userId)
            {
                return Forbid();
            }

            await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Notification marked as read"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.BadRequest("Failed to mark notification as read"));
        }
    }

    /// <summary>
    /// Mark all notifications as read (Admin/Lecturer/Student)
    /// </summary>
    [HttpPost("mark-all-read")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead()
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        try
        {
            await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "All notifications marked as read"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.BadRequest("Failed to mark all notifications as read"));
        }
    }
}
