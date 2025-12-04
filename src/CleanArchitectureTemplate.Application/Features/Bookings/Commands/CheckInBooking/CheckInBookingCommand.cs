using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CheckInBooking;

public class CheckInBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; set; }
}
