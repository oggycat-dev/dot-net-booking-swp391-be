using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Models;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetApprovedBookings;

/// <summary>
/// Query to get all approved bookings (for admin to view cancellable bookings)
/// </summary>
public record GetApprovedBookingsQuery : IRequest<PaginatedResult<BookingDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? FacilityId { get; init; }
    public Guid? CampusId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SearchTerm { get; init; }
}
