using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;
using CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.ApproveCampusChange;
using CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.RequestCampusChange;
using CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Queries.GetMyCampusChangeRequests;
using CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Queries.GetPendingCampusChangeRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Campus change request management controller
/// </summary>
[ApiController]
[Route("api/campus-change-requests")]
[Authorize]
public class CampusChangeRequestController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampusChangeRequestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Request campus change (Student/Lecturer only)
    /// </summary>
    /// <param name="request">Campus change request details</param>
    /// <returns>Created campus change request</returns>
    [HttpPost]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<CampusChangeRequestDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<CampusChangeRequestDto>>> RequestCampusChange(
        [FromBody] RequestCampusChangeRequest request)
    {
        var command = new RequestCampusChangeCommand
        {
            RequestedCampusId = request.RequestedCampusId,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        var response = ApiResponse<CampusChangeRequestDto>.Created(result, "Campus change request submitted successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get my campus change requests history (Student/Lecturer only)
    /// </summary>
    /// <returns>List of user's campus change requests</returns>
    [HttpGet("my-requests")]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<List<MyCampusChangeRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<List<MyCampusChangeRequestDto>>>> GetMyRequests()
    {
        var requests = await _mediator.Send(new GetMyCampusChangeRequestsQuery());
        var response = ApiResponse<List<MyCampusChangeRequestDto>>.Ok(requests, "Campus change requests retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get pending campus change requests (Admin only)
    /// </summary>
    /// <returns>List of pending campus change requests</returns>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<CampusChangeRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<List<CampusChangeRequestDto>>>> GetPendingRequests()
    {
        var requests = await _mediator.Send(new GetPendingCampusChangeRequestsQuery());
        var response = ApiResponse<List<CampusChangeRequestDto>>.Ok(requests, "Pending campus change requests retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Approve or reject campus change request (Admin only)
    /// </summary>
    /// <param name="requestId">Campus change request ID</param>
    /// <param name="request">Approval decision</param>
    /// <returns>Success message</returns>
    [HttpPost("{requestId}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> ApproveRequest(
        Guid requestId,
        [FromBody] ApproveCampusChangeRequest request)
    {
        var command = new ApproveCampusChangeCommand
        {
            RequestId = requestId,
            Approved = request.Approved,
            Comment = request.Comment
        };

        await _mediator.Send(command);
        
        var message = request.Approved 
            ? "Campus change request approved successfully" 
            : "Campus change request rejected";
            
        var response = ApiResponse<object>.Ok(null, message);
        return Ok(response);
    }
}
