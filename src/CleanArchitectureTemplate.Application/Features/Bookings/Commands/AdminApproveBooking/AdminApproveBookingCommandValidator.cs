using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.AdminApproveBooking;

public class AdminApproveBookingCommandValidator : AbstractValidator<AdminApproveBookingCommand>
{
    public AdminApproveBookingCommandValidator()
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
