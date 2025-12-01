using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using CleanArchitectureTemplate.Application.Common.Models;
using CleanArchitectureTemplate.Application.Features.Users.Commands.CreateUser;
using CleanArchitectureTemplate.Application.Features.Users.Commands.DeleteUser;
using CleanArchitectureTemplate.Application.Features.Users.Commands.UpdateUser;
using CleanArchitectureTemplate.Application.Features.Users.Queries.GetUserById;
using CleanArchitectureTemplate.Application.Features.Users.Queries.GetUsers;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserDto = CleanArchitectureTemplate.Application.Common.DTOs.Users.UserDto;

namespace CleanArchitectureTemplate.API.Controllers.CMS;

/// <summary>
/// User management controller for CMS
/// </summary>
[ApiController]
[Route("api/cms/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination</param>
    /// <returns>List of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<UserDto>>>> GetAll([FromQuery] GetUsersQuery query)
    {
        var users = await _mediator.Send(query);
        var response = ApiResponse<PaginatedResult<UserDto>>.Ok(users, "Users retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        var response = ApiResponse<UserDto>.Ok(user, "User retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="command">User creation information</param>
    /// <returns>Created user information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success && result.Data != null)
        {
            var response = ApiResponse<UserDto>.Created(result.Data, "User created successfully");
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, response);
        }
        
        var errorResponse = ApiResponse<object>.BadRequest(result.Message, result.Errors);
        return StatusCode(errorResponse.StatusCode, errorResponse);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">User update information</param>
    /// <returns>Updated user information</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<UserDto>>> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            var errorResponse = ApiResponse<object>.BadRequest("User ID in URL does not match the ID in request body");
            return StatusCode(errorResponse.StatusCode, errorResponse);
        }

        var user = await _mediator.Send(command);
        var response = ApiResponse<UserDto>.Ok(user, "User updated successfully");
        return Ok(response);
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        var response = ApiResponse<object>.Ok(null, "User deleted successfully");
        return Ok(response);
    }
}