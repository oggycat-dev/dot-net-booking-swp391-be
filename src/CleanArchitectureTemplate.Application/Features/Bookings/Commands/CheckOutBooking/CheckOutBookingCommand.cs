using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CheckOutBooking;

public class CheckOutBookingCommand : IRequest<Unit>
{
    public Guid BookingId { get; set; }
}
