using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using CleanArchitectureTemplate.Application.Features.Campuses.Commands.CreateCampus;
using CleanArchitectureTemplate.Application.Features.Campuses.Commands.DeleteCampus;
using CleanArchitectureTemplate.Application.Features.Campuses.Commands.UpdateCampus;
using CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetAllCampuses;
using CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetCampusByCode;
using CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetCampusById;
using CleanArchitectureTemplate.API.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Campus management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CampusController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all active campuses for selection
    /// </summary>
    /// <returns>List of active campuses</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<CampusDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CampusDto>>>> GetAllCampuses()
    {
        var campuses = await _mediator.Send(new GetAllCampusesQuery());
        var response = ApiResponse<List<CampusDto>>.Ok(campuses, "Campuses retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get campus by ID
    /// </summary>
    /// <param name="id">Campus ID</param>
    /// <returns>Campus details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CampusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CampusDto>>> GetCampusById(Guid id)
    {
        var campus = await _mediator.Send(new GetCampusByIdQuery(id));

        if (campus == null)
        {
            var errorResponse = ApiResponse<object>.NotFound("Campus not found");
            return NotFound(errorResponse);
        }

        var response = ApiResponse<CampusDto>.Ok(campus, "Campus retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get campus by code
    /// </summary>
    /// <param name="code">Campus code</param>
    /// <returns>Campus details</returns>
    [HttpGet("code/{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CampusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CampusDto>>> GetCampusByCode(string code)
    {
        var campus = await _mediator.Send(new GetCampusByCodeQuery(code));

        if (campus == null)
        {
            var errorResponse = ApiResponse<object>.NotFound("Campus not found");
            return NotFound(errorResponse);
        }

        var response = ApiResponse<CampusDto>.Ok(campus, "Campus retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Create a new campus (Admin only)
    /// </summary>
    /// <param name="request">Campus creation request</param>
    /// <returns>Created campus</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampusDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<CampusDto>>> CreateCampus([FromBody] CreateCampusRequest request)
    {
        var command = new CreateCampusCommand
        {
            CampusCode = request.CampusCode,
            CampusName = request.CampusName,
            Address = request.Address,
            WorkingHoursStart = request.WorkingHoursStart,
            WorkingHoursEnd = request.WorkingHoursEnd,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail
        };

        var campus = await _mediator.Send(command);
        var response = ApiResponse<CampusDto>.Created(campus, "Campus created successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update an existing campus (Admin only)
    /// </summary>
    /// <param name="id">Campus ID</param>
    /// <param name="request">Campus update request</param>
    /// <returns>Updated campus</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CampusDto>>> UpdateCampus(Guid id, [FromBody] UpdateCampusRequest request)
    {
        var command = new UpdateCampusCommand
        {
            Id = id,
            CampusName = request.CampusName,
            Address = request.Address,
            WorkingHoursStart = request.WorkingHoursStart,
            WorkingHoursEnd = request.WorkingHoursEnd,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail,
            IsActive = request.IsActive
        };

        var campus = await _mediator.Send(command);
        var response = ApiResponse<CampusDto>.Ok(campus, "Campus updated successfully");
        return Ok(response);
    }

    /// <summary>
    /// Delete a campus (Admin only)
    /// </summary>
    /// <param name="id">Campus ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCampus(Guid id)
    {
        await _mediator.Send(new DeleteCampusCommand(id));
        var response = ApiResponse<object>.Ok(null, "Campus deleted successfully");
        return Ok(response);
    }
}
