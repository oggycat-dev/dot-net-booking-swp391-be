using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CancelBooking;

/// <summary>
/// Validator for CancelBookingCommand
/// </summary>
public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required")
            .MaximumLength(500)
            .WithMessage("Cancellation reason must not exceed 500 characters");
    }
}
