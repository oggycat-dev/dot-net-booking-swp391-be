using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Features.Users.Queries.GetMyProfile;
using CleanArchitectureTemplate.Application.Features.Users.Commands.UpdateMyProfile;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserDto = CleanArchitectureTemplate.Application.Common.DTOs.Users.UserDto;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// User profile controller for Students and Lecturers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Lecturer)}")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>User profile information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetMyProfile()
    {
        var profile = await _mediator.Send(new GetMyProfileQuery());
        var response = ApiResponse<UserDto>.Ok(profile, "Profile retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="command">Profile update information</param>
    /// <returns>Success message</returns>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
    {
        await _mediator.Send(command);
        var response = ApiResponse<object>.Ok(null, "Profile updated successfully");
        return Ok(response);
    }
}
