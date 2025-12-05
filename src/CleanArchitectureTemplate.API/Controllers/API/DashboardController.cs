using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Dashboard;
using CleanArchitectureTemplate.Application.Features.Dashboard.Queries.GetAdminDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Dashboard controller for admin statistics
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get admin dashboard statistics
    /// </summary>
    /// <returns>Dashboard statistics including users, bookings, and facilities metrics</returns>
    [HttpGet("admin")]
    [ProducesResponseType(typeof(ApiResponse<AdminDashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<AdminDashboardDto>>> GetAdminDashboard()
    {
        var dashboard = await _mediator.Send(new GetAdminDashboardQuery());
        var response = ApiResponse<AdminDashboardDto>.Ok(dashboard, "Admin dashboard statistics retrieved successfully");
        return Ok(response);
    }
}
