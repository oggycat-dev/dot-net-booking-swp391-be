using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetMyPendingBookings;

public record GetMyPendingBookingsQuery : IRequest<List<BookingDto>>;
