using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Features.Facilities.Commands.CreateFacility;
using CleanArchitectureTemplate.Application.Features.Facilities.Commands.DeleteFacility;
using CleanArchitectureTemplate.Application.Features.Facilities.Commands.UpdateFacility;
using CleanArchitectureTemplate.Application.Features.Facilities.Queries.GetAllFacilities;
using CleanArchitectureTemplate.Application.Features.Facilities.Queries.GetFacilityById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Facility management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FacilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all facilities with optional filters
    /// </summary>
    /// <param name="campusId">Filter by campus (optional)</param>
    /// <param name="facilityTypeId">Filter by facility type (optional)</param>
    /// <param name="availableOnly">Show only available facilities (optional)</param>
    /// <returns>List of facilities</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<FacilityDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<FacilityDto>>>> GetAllFacilities(
        [FromQuery] Guid? campusId = null,
        [FromQuery] Guid? facilityTypeId = null,
        [FromQuery] bool? availableOnly = null)
    {
        var facilities = await _mediator.Send(new GetAllFacilitiesQuery(campusId, facilityTypeId, availableOnly));
        var response = ApiResponse<List<FacilityDto>>.Ok(facilities, "Facilities retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get facility by ID
    /// </summary>
    /// <param name="id">Facility ID</param>
    /// <returns>Facility details</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<FacilityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> GetFacilityById(Guid id)
    {
        var facility = await _mediator.Send(new GetFacilityByIdQuery(id));

        if (facility == null)
        {
            var errorResponse = ApiResponse<object>.NotFound("Facility not found");
            return NotFound(errorResponse);
        }

        var response = ApiResponse<FacilityDto>.Ok(facility, "Facility retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Create a new facility (Admin only)
    /// </summary>
    /// <param name="request">Facility creation request</param>
    /// <returns>Created facility</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> CreateFacility([FromBody] CreateFacilityRequest request)
    {
        var command = new CreateFacilityCommand
        {
            FacilityCode = request.FacilityCode,
            FacilityName = request.FacilityName,
            TypeId = request.TypeId,
            CampusId = request.CampusId,
            Building = request.Building,
            Floor = request.Floor,
            RoomNumber = request.RoomNumber,
            Capacity = request.Capacity,
            Description = request.Description,
            Equipment = request.Equipment,
            ImageUrl = request.ImageUrl
        };

        var facility = await _mediator.Send(command);
        var response = ApiResponse<FacilityDto>.Created(facility, "Facility created successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update a facility (Admin only)
    /// </summary>
    /// <param name="id">Facility ID</param>
    /// <param name="request">Facility update request</param>
    /// <returns>Updated facility</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> UpdateFacility(Guid id, [FromBody] UpdateFacilityRequest request)
    {
        var command = new UpdateFacilityCommand
        {
            Id = id,
            FacilityName = request.FacilityName,
            TypeId = request.TypeId,
            Building = request.Building,
            Floor = request.Floor,
            RoomNumber = request.RoomNumber,
            Capacity = request.Capacity,
            Description = request.Description,
            Equipment = request.Equipment,
            ImageUrl = request.ImageUrl,
            Status = request.Status,
            IsActive = request.IsActive
        };

        var facility = await _mediator.Send(command);
        var response = ApiResponse<FacilityDto>.Ok(facility, "Facility updated successfully");
        return Ok(response);
    }

    /// <summary>
    /// Delete a facility (Admin only)
    /// </summary>
    /// <param name="id">Facility ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFacility(Guid id)
    {
        await _mediator.Send(new DeleteFacilityCommand(id));
        var response = ApiResponse<object>.Ok(null, "Facility deleted successfully");
        return Ok(response);
    }
}
