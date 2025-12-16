using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CancelBooking;

/// <summary>
/// Command to cancel a booking (Admin only)
/// </summary>
public record CancelBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
