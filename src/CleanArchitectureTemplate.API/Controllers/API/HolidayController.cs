using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Holiday;
using CleanArchitectureTemplate.Application.Features.Holidays.Commands.CreateHoliday;
using CleanArchitectureTemplate.Application.Features.Holidays.Commands.DeleteHoliday;
using CleanArchitectureTemplate.Application.Features.Holidays.Queries.GetAllHolidays;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Holiday management controller
/// </summary>
[ApiController]
[Route("api/holidays")]
[Authorize]
public class HolidayController : ControllerBase
{
    private readonly IMediator _mediator;

    public HolidayController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all holidays (All roles)
    /// </summary>
    /// <returns>List of holidays</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<HolidayDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<List<HolidayDto>>>> GetAllHolidays()
    {
        var holidays = await _mediator.Send(new GetAllHolidaysQuery());
        var response = ApiResponse<List<HolidayDto>>.Ok(holidays, "Holidays retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Create a new holiday (Admin only)
    /// </summary>
    /// <param name="request">Holiday creation request</param>
    /// <returns>Created holiday</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<HolidayDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<HolidayDto>>> CreateHoliday(
        [FromBody] CreateHolidayRequest request)
    {
        var command = new CreateHolidayCommand
        {
            HolidayName = request.HolidayName,
            HolidayDate = request.HolidayDate,
            IsRecurring = request.IsRecurring,
            Description = request.Description
        };

        var result = await _mediator.Send(command);
        var response = ApiResponse<HolidayDto>.Created(result, "Holiday created successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Delete a holiday (Admin only)
    /// </summary>
    /// <param name="holidayId">Holiday ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{holidayId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteHoliday(Guid holidayId)
    {
        var command = new DeleteHolidayCommand { HolidayId = holidayId };
        await _mediator.Send(command);

        var response = ApiResponse<object>.Ok(null, "Holiday deleted successfully");
        return Ok(response);
    }
}
