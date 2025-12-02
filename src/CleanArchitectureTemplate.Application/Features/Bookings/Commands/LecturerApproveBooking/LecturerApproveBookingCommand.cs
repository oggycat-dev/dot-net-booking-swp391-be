using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.LecturerApproveBooking;

public record LecturerApproveBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; init; }
    public bool Approved { get; init; }
    public string? Comment { get; init; }
}
