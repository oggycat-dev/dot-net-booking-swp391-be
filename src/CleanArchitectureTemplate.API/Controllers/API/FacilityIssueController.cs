using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.ChangeRoomForIssue;
using CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.CreateIssueReport;
using CleanArchitectureTemplate.Application.Features.FacilityIssues.Queries.GetMyIssueReports;
using CleanArchitectureTemplate.Application.Features.FacilityIssues.Queries.GetPendingIssueReports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Facility issue report controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FacilityIssueController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacilityIssueController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get my facility issue reports (Student/Lecturer)
    /// </summary>
    /// <returns>List of my issue reports</returns>
    [HttpGet("my-reports")]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<List<FacilityIssueReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<List<FacilityIssueReportDto>>>> GetMyIssueReports()
    {
        var reports = await _mediator.Send(new GetMyIssueReportsQuery());
        var response = ApiResponse<List<FacilityIssueReportDto>>.Ok(reports, "Issue reports retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get pending facility issue reports (Admin only)
    /// </summary>
    /// <returns>List of pending issue reports</returns>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<FacilityIssueReportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<List<FacilityIssueReportDto>>>> GetPendingIssueReports()
    {
        var reports = await _mediator.Send(new GetPendingIssueReportsQuery());
        var response = ApiResponse<List<FacilityIssueReportDto>>.Ok(reports, "Pending issue reports retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Report a facility issue during booking (Student/Lecturer)
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <param name="issueTitle">Issue title</param>
    /// <param name="issueDescription">Issue description</param>
    /// <param name="severity">Severity level (Low, Medium, High, Critical)</param>
    /// <param name="category">Issue category (Leak, Equipment, Cleanliness, Safety, Other)</param>
    /// <param name="images">Evidence images (optional, multiple files)</param>
    /// <returns>Created issue report</returns>
    [HttpPost("report")]
    [Authorize(Roles = "Student,Lecturer")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<FacilityIssueReportDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<FacilityIssueReportDto>>> CreateIssueReport(
        [FromForm] Guid bookingId,
        [FromForm] string issueTitle,
        [FromForm] string issueDescription,
        [FromForm] string severity,
        [FromForm] string category,
        [FromForm] List<IFormFile>? images)
    {
        var command = new CreateIssueReportCommand
        {
            BookingId = bookingId,
            IssueTitle = issueTitle,
            IssueDescription = issueDescription,
            Severity = severity,
            Category = category,
            Images = images
        };

        var report = await _mediator.Send(command);
        var response = ApiResponse<FacilityIssueReportDto>.Created(report, "Issue report created successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Change room for a reported issue (Admin only)
    /// </summary>
    /// <param name="reportId">Issue report ID</param>
    /// <param name="request">Change room request</param>
    /// <returns>Updated issue report</returns>
    [HttpPost("{reportId}/change-room")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityIssueReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FacilityIssueReportDto>>> ChangeRoomForIssue(
        Guid reportId,
        [FromBody] ChangeRoomForIssueRequest request)
    {
        var command = new ChangeRoomForIssueCommand
        {
            ReportId = reportId,
            NewFacilityId = request.NewFacilityId,
            AdminResponse = request.AdminResponse
        };

        var report = await _mediator.Send(command);
        var response = ApiResponse<FacilityIssueReportDto>.Ok(report, "Room changed successfully. Email notification sent to user.");
        return Ok(response);
    }
}
