using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.AdminApproveBooking;

public record AdminApproveBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; init; }
    public bool Approved { get; init; }
    public string? Comment { get; init; }
}
