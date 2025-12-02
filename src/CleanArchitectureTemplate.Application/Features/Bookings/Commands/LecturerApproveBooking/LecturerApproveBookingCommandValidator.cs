using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.LecturerApproveBooking;

public class LecturerApproveBookingCommandValidator : AbstractValidator<LecturerApproveBookingCommand>
{
    public LecturerApproveBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required");

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .WithMessage("Comment must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}
