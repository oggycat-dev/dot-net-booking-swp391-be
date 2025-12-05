using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetBookingsForCalendar;

/// <summary>
/// Query to get bookings for calendar view
/// Shows all bookings that occupy time slots (pending, approved, in-use)
/// </summary>
public record GetBookingsForCalendarQuery : IRequest<List<BookingCalendarDto>>
{
    /// <summary>
    /// Start date of the week
    /// </summary>
    public DateTime StartDate { get; init; }
    
    /// <summary>
    /// End date of the week
    /// </summary>
    public DateTime EndDate { get; init; }
    
    /// <summary>
    /// Filter by facility (optional)
    /// </summary>
    public Guid? FacilityId { get; init; }
    
    /// <summary>
    /// Filter by campus (optional)
    /// </summary>
    public Guid? CampusId { get; init; }
}
