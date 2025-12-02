using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetPendingAdminApprovals;

public record GetPendingAdminApprovalsQuery : IRequest<List<BookingListDto>>;
