using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Features.Facilities.Commands.CreateFacility;
using CleanArchitectureTemplate.Application.Features.Facilities.Commands.DeleteFacility;
using CleanArchitectureTemplate.Application.Features.Facilities.Commands.UpdateFacility;
using CleanArchitectureTemplate.Application.Features.Facilities.Commands.UploadFacilityImages;
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
    /// <param name="facilityCode">Facility code</param>
    /// <param name="facilityName">Facility name</param>
    /// <param name="typeId">Facility type ID</param>
    /// <param name="campusId">Campus ID</param>
    /// <param name="building">Building</param>
    /// <param name="floor">Floor</param>
    /// <param name="roomNumber">Room number</param>
    /// <param name="capacity">Capacity</param>
    /// <param name="description">Description</param>
    /// <param name="equipment">Equipment</param>
    /// <param name="images">Facility images (multiple files)</param>
    /// <returns>Created facility</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> CreateFacility(
        [FromForm] string facilityCode,
        [FromForm] string facilityName,
        [FromForm] Guid typeId,
        [FromForm] Guid campusId,
        [FromForm] string? building,
        [FromForm] string? floor,
        [FromForm] string? roomNumber,
        [FromForm] int capacity,
        [FromForm] string? description,
        [FromForm] string? equipment,
        [FromForm] List<IFormFile>? images)
    {
        var command = new CreateFacilityCommand
        {
            FacilityCode = facilityCode,
            FacilityName = facilityName,
            TypeId = typeId,
            CampusId = campusId,
            Building = building,
            Floor = floor,
            RoomNumber = roomNumber,
            Capacity = capacity,
            Description = description,
            Equipment = equipment,
            Images = images
        };

        var facility = await _mediator.Send(command);
        var response = ApiResponse<FacilityDto>.Created(facility, "Facility created successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update a facility (Admin only)
    /// </summary>
    /// <param name="id">Facility ID</param>
    /// <param name="facilityName">Facility name</param>
    /// <param name="typeId">Facility type ID</param>
    /// <param name="building">Building</param>
    /// <param name="floor">Floor</param>
    /// <param name="roomNumber">Room number</param>
    /// <param name="capacity">Capacity</param>
    /// <param name="description">Description</param>
    /// <param name="equipment">Equipment</param>
    /// <param name="imageUrl">Image URL (optional, if not uploading new images)</param>
    /// <param name="images">New images to upload (optional)</param>
    /// <param name="status">Status</param>
    /// <param name="isActive">Is active</param>
    /// <returns>Updated facility</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacilityDto>>> UpdateFacility(
        Guid id,
        [FromForm] string facilityName,
        [FromForm] Guid typeId,
        [FromForm] string? building,
        [FromForm] string? floor,
        [FromForm] string? roomNumber,
        [FromForm] int capacity,
        [FromForm] string? description,
        [FromForm] string? equipment,
        [FromForm] string? imageUrl,
        [FromForm] List<IFormFile>? images,
        [FromForm] string status,
        [FromForm] bool isActive)
    {
        var command = new UpdateFacilityCommand
        {
            Id = id,
            FacilityName = facilityName,
            TypeId = typeId,
            Building = building,
            Floor = floor,
            RoomNumber = roomNumber,
            Capacity = capacity,
            Description = description,
            Equipment = equipment,
            ImageUrl = imageUrl,
            Images = images,
            Status = status,
            IsActive = isActive
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

    /// <summary>
    /// Upload multiple images for a facility (Admin only)
    /// </summary>
    /// <param name="id">Facility ID</param>
    /// <param name="images">Image files</param>
    /// <returns>List of uploaded image URLs</returns>
    [HttpPost("{id}/upload-images")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<string>>>> UploadFacilityImages(
        Guid id, 
        [FromForm] List<IFormFile> images)
    {
        var command = new UploadFacilityImagesCommand
        {
            FacilityId = id,
            Images = images
        };

        var imageUrls = await _mediator.Send(command);
        var response = ApiResponse<List<string>>.Ok(imageUrls, $"{imageUrls.Count} image(s) uploaded successfully");
        return Ok(response);
    }
}
