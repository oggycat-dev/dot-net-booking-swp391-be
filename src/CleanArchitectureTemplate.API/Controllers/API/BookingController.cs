using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Features.Bookings.Commands.AdminApproveBooking;
using CleanArchitectureTemplate.Application.Features.Bookings.Commands.CheckInBooking;
using CleanArchitectureTemplate.Application.Features.Bookings.Commands.CheckOutBooking;
using CleanArchitectureTemplate.Application.Features.Bookings.Commands.CreateBooking;
using CleanArchitectureTemplate.Application.Features.Bookings.Commands.LecturerApproveBooking;
using CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetBookingsForCalendar;
using CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetMyBookingHistory;
using CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetMyPendingBookings;
using CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetPendingAdminApprovals;
using CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetPendingLecturerApprovals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureTemplate.API.Controllers.API;

/// <summary>
/// Booking management controller
/// </summary>
[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get bookings for calendar view - shows occupied time slots
    /// </summary>
    /// <param name="startDate">Start date of the week (YYYY-MM-DD)</param>
    /// <param name="endDate">End date of the week (YYYY-MM-DD)</param>
    /// <param name="facilityId">Filter by facility (optional)</param>
    /// <param name="campusId">Filter by campus (optional)</param>
    /// <returns>List of bookings in the specified time range</returns>
    [HttpGet("calendar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<BookingCalendarDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<List<BookingCalendarDto>>>> GetBookingsForCalendar(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? facilityId = null,
        [FromQuery] Guid? campusId = null)
    {
        if (startDate > endDate)
        {
            return BadRequest(ApiResponse<object>.BadRequest("StartDate must be before or equal to EndDate"));
        }

        var query = new GetBookingsForCalendarQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            FacilityId = facilityId,
            CampusId = campusId
        };

        var bookings = await _mediator.Send(query);
        var response = ApiResponse<List<BookingCalendarDto>>.Ok(
            bookings, 
            $"Retrieved {bookings.Count} booking(s) for the specified period");
        return Ok(response);
    }

    /// <summary>
    /// Create a new booking (Student/Lecturer)
    /// </summary>
    /// <param name="request">Booking creation request</param>
    /// <returns>Created booking</returns>
    [HttpPost]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateBooking(
        [FromBody] CreateBookingRequest request)
    {
        // Parse time strings to TimeSpan
        if (!TimeSpan.TryParse(request.StartTime, out var startTime))
        {
            return BadRequest(ApiResponse<object>.BadRequest("Invalid StartTime format. Use HH:mm:ss"));
        }

        if (!TimeSpan.TryParse(request.EndTime, out var endTime))
        {
            return BadRequest(ApiResponse<object>.BadRequest("Invalid EndTime format. Use HH:mm:ss"));
        }

        var command = new CreateBookingCommand
        {
            FacilityId = request.FacilityId,
            BookingDate = request.BookingDate,
            StartTime = startTime,
            EndTime = endTime,
            Purpose = request.Purpose,
            NumParticipants = request.NumParticipants,
            EquipmentNeeded = request.EquipmentNeeded,
            Note = request.Note,
            LecturerEmail = request.LecturerEmail
        };

        var result = await _mediator.Send(command);
        var response = ApiResponse<BookingDto>.Created(result, "Booking created successfully");
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get my pending bookings (Student/Lecturer)
    /// </summary>
    /// <returns>List of my bookings that are pending approval</returns>
    [HttpGet("my-pending")]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<List<BookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<List<BookingDto>>>> GetMyPendingBookings()
    {
        var bookings = await _mediator.Send(new GetMyPendingBookingsQuery());
        var response = ApiResponse<List<BookingDto>>.Ok(bookings, "My pending bookings retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get my booking history (Student/Lecturer)
    /// </summary>
    /// <returns>List of all my bookings</returns>
    [HttpGet("my-history")]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<List<BookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<List<BookingDto>>>> GetMyBookingHistory()
    {
        var bookings = await _mediator.Send(new GetMyBookingHistoryQuery());
        var response = ApiResponse<List<BookingDto>>.Ok(bookings, "My booking history retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Get bookings waiting for lecturer approval (Lecturer only)
    /// </summary>
    /// <returns>List of bookings waiting for lecturer approval</returns>
    [HttpGet("pending-lecturer-approval")]
    [Authorize(Roles = "Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<List<BookingListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<List<BookingListDto>>>> GetPendingLecturerApprovals()
    {
        var bookings = await _mediator.Send(new GetPendingLecturerApprovalsQuery());
        var response = ApiResponse<List<BookingListDto>>.Ok(bookings, "Pending bookings retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Lecturer approve or reject student booking (Lecturer only)
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <param name="request">Approval decision</param>
    /// <returns>Success message</returns>
    [HttpPost("{bookingId}/lecturer-approve")]
    [Authorize(Roles = "Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> LecturerApproveBooking(
        Guid bookingId,
        [FromBody] ApproveBookingRequest request)
    {
        var command = new LecturerApproveBookingCommand
        {
            BookingId = bookingId,
            Approved = request.Approved,
            Comment = request.Comment
        };

        await _mediator.Send(command);

        var message = request.Approved 
            ? "Booking approved by lecturer and sent to admin for final approval" 
            : "Booking rejected by lecturer";

        var response = ApiResponse<object>.Ok(null, message);
        return Ok(response);
    }

    /// <summary>
    /// Get bookings waiting for admin approval (Admin only)
    /// </summary>
    /// <returns>List of bookings waiting for admin approval</returns>
    [HttpGet("pending-admin-approval")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<BookingListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<List<BookingListDto>>>> GetPendingAdminApprovals()
    {
        var bookings = await _mediator.Send(new GetPendingAdminApprovalsQuery());
        var response = ApiResponse<List<BookingListDto>>.Ok(bookings, "Pending bookings retrieved successfully");
        return Ok(response);
    }

    /// <summary>
    /// Admin approve or reject booking (Admin only)
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <param name="request">Approval decision</param>
    /// <returns>Success message</returns>
    [HttpPost("{bookingId}/admin-approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> AdminApproveBooking(
        Guid bookingId,
        [FromBody] ApproveBookingRequest request)
    {
        var command = new AdminApproveBookingCommand
        {
            BookingId = bookingId,
            Approved = request.Approved,
            Comment = request.Comment
        };

        await _mediator.Send(command);

        var message = request.Approved 
            ? "Booking approved successfully" 
            : "Booking rejected";

        var response = ApiResponse<object>.Ok(null, message);
        return Ok(response);
    }

    /// <summary>
    /// Check-in to a booking (Student/Lecturer)
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{bookingId}/check-in")]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> CheckInBooking(Guid bookingId)
    {
        var command = new CheckInBookingCommand { BookingId = bookingId };
        await _mediator.Send(command);

        var response = ApiResponse<object>.Ok(null, "Checked in successfully");
        return Ok(response);
    }

    /// <summary>
    /// Check-out from a booking (Student/Lecturer)
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{bookingId}/check-out")]
    [Authorize(Roles = "Student,Lecturer")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> CheckOutBooking(Guid bookingId)
    {
        var command = new CheckOutBookingCommand { BookingId = bookingId };
        await _mediator.Send(command);

        var response = ApiResponse<object>.Ok(null, "Checked out successfully");
        return Ok(response);
    }
}
