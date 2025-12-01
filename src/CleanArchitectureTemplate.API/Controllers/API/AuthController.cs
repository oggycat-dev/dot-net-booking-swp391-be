using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Auth;
using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using CleanArchitectureTemplate.Application.Features.Users.Commands.CreateUser;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserDto = CleanArchitectureTemplate.Application.Common.DTOs.Users.UserDto;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthenticationService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        IMediator mediator,
        IAuthenticationService authService,
        ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _authService = authService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">Registration information</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterRequest request)
    {
        var command = new CreateUserCommand
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = UserRole.Student.ToString() // Default role for registration
        };

        var result = await _mediator.Send(command);
        if (result.Success && result.Data != null)
        {
            var response = ApiResponse<UserDto>.Created(result.Data, "User registered successfully");
            return StatusCode(response.StatusCode, response);
        }
        
        var errorResponse = ApiResponse<object>.BadRequest(result.Message, result.Errors);
        return StatusCode(errorResponse.StatusCode, errorResponse);
    }

    /// <summary>
    /// Login to the system
    /// </summary>
    /// <param name="request">Login credentials (username is email address)</param>
    /// <returns>JWT tokens</returns>
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