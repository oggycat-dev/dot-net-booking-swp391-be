using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetPendingLecturerApprovals;

public record GetPendingLecturerApprovalsQuery : IRequest<List<BookingListDto>>;
