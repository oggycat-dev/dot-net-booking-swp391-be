using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;
using CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.CreateFacilityType;
using CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.UpdateFacilityType;
using CleanArchitectureTemplate.Application.Features.FacilityTypes.Queries.GetAllFacilityTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Facility type management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FacilityTypeController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacilityTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all facility types
    /// </summary>
    /// <param name="activeOnly">Show only active types (optional)</param>
    /// <returns>List of facility types</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<FacilityTypeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<FacilityTypeDto>>>> GetAllFacilityTypes([FromQuery] bool? activeOnly = null)
    {
        var facilityTypes = await _mediator.Send(new GetAllFacilityTypesQuery(activeOnly));
        var response = ApiResponse<List<FacilityTypeDto>>.Ok(facilityTypes, "Facility types retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Create a new facility type (Admin only)
    /// </summary>
    /// <param name="request">Facility type creation request</param>
    /// <returns>Created facility type</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityTypeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<FacilityTypeDto>>> CreateFacilityType([FromBody] CreateFacilityTypeRequest request)
    {
        var command = new CreateFacilityTypeCommand
        {
            TypeCode = request.TypeCode,
            TypeName = request.TypeName,
            Description = request.Description
        };

        var facilityType = await _mediator.Send(command);
        var response = ApiResponse<FacilityTypeDto>.Created(facilityType, "Facility type created successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update a facility type (Admin only)
    /// </summary>
    /// <param name="id">Facility type ID</param>
    /// <param name="request">Facility type update request</param>
    /// <returns>Updated facility type</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityTypeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacilityTypeDto>>> UpdateFacilityType(Guid id, [FromBody] UpdateFacilityTypeRequest request)
    {
        var command = new UpdateFacilityTypeCommand
        {
            Id = id,
            TypeName = request.TypeName,
            Description = request.Description,
            IsActive = request.IsActive
        };

        var facilityType = await _mediator.Send(command);
        var response = ApiResponse<FacilityTypeDto>.Ok(facilityType, "Facility type updated successfully");
        return Ok(response);
    }
}
