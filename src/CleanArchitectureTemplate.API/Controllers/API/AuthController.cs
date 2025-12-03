using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Auth;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.ApproveRegistration;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.ForgotPassword;
using CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPasswordWithCode;
using CleanArchitectureTemplate.Application.Features.Auth.Queries.GetPendingRegistrations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public AuthController(
        IAuthenticationService authService,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user account (Student/Lecturer) - requires admin approval
    /// </summary>
    /// <param name="request">Registration information including campus and role selection</param>
    /// <returns>Registration submission confirmation</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.FullName,
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.PhoneNumber,
            request.CampusId,
            request.Role,
            request.Department,
            request.Major
        );

        var response = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Login to the system with @fpt.edu.vn email
    /// </summary>
    /// <param name="request">Login credentials (email must be @fpt.edu.vn)</param>
    /// <returns>JWT tokens and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var loginResponse = await _authService.LoginAsync(request);
        var response = ApiResponse<LoginResponse>.Ok(loginResponse, "Login successful");
        return Ok(response);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New JWT tokens</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var loginResponse = await _authService.RefreshTokenAsync(request);
        var response = ApiResponse<LoginResponse>.Ok(loginResponse, "Token refreshed successfully");
        return Ok(response);
    }

    /// <summary>
    /// Change current user password
    /// </summary>
    /// <param name="request">Current and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        await _authService.ChangePasswordAsync(userId, request);
        var response = ApiResponse<object>.Ok(null, "Password changed successfully");
        return Ok(response);
    }

    /// <summary>
    /// Logout from the system
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        await _authService.LogoutAsync(_currentUserService.UserId?.ToString() ?? string.Empty);
        var response = ApiResponse<object>.Ok(null, "Logout successful");
        return Ok(response);
    }

    /// <summary>
    /// Get pending registrations for admin approval
    /// </summary>
    /// <returns>List of pending registrations</returns>
    [HttpGet("pending-registrations")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<PendingRegistrationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<PendingRegistrationDto>>>> GetPendingRegistrations()
    {
        var pendingRegistrations = await _mediator.Send(new GetPendingRegistrationsQuery());
        var response = ApiResponse<List<PendingRegistrationDto>>.Ok(pendingRegistrations, "Pending registrations retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Approve or reject a registration
    /// </summary>
    /// <param name="request">Approval decision</param>
    /// <returns>Success message</returns>
    [HttpPost("approve-registration")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> ApproveRegistration([FromBody] ApproveRegistrationRequest request)
    {
        var command = new ApproveRegistrationCommand(
            request.UserId,
            request.IsApproved,
            request.RejectionReason
        );

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Request password reset - sends verification code to email
    /// </summary>
    /// <param name="request">Email address</param>
    /// <returns>Success message</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _mediator.Send(new ForgotPasswordCommand(request.Email));
        var response = ApiResponse<object>.Ok(null, "If the email exists, a verification code has been sent.");
        return Ok(response);
    }

    /// <summary>
    /// Reset password using verification code
    /// </summary>
    /// <param name="request">Email, verification code and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _mediator.Send(new ResetPasswordWithCodeCommand(
            request.Email,
            request.VerificationCode,
            request.NewPassword
        ));
        var response = ApiResponse<object>.Ok(null, "Password has been reset successfully.");
        return Ok(response);
    }
}
