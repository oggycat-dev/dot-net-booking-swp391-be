using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Notifications;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Notification controller for Firebase Cloud Messaging
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
}
