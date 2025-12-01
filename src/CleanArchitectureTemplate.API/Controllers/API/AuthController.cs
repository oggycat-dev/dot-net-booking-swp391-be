using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Auth;
using CleanArchitectureTemplate.Application.Common.Interfaces;
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

    public AuthController(
        IAuthenticationService authService,
        ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Register a new user account with @fpt.edu.vn email
    /// </summary>
    /// <param name="request">Registration information including campus selection</param>
    /// <returns>Login response with JWT tokens</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterRequest request)
    {
        var loginResponse = await _authService.RegisterAsync(request);
        var response = ApiResponse<LoginResponse>.Created(loginResponse, "User registered successfully");
        return StatusCode(response.StatusCode, response);
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
}