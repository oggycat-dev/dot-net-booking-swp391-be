using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetMyBookingHistory;

public class GetMyBookingHistoryQuery : IRequest<List<BookingDto>>
{
}
