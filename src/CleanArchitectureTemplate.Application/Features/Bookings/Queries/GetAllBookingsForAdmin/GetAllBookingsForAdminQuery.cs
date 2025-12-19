using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Models;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetAllBookingsForAdmin;

/// <summary>
/// Query to get all bookings with filters for admin management
/// </summary>
public record GetAllBookingsForAdminQuery : IRequest<PaginatedResult<BookingDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? FacilityId { get; init; }
    public Guid? CampusId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? Status { get; init; }  // Filter by booking status
    public string? SearchTerm { get; init; }
}
