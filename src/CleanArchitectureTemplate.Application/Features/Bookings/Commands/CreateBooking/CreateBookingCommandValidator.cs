using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty()
            .WithMessage("Facility ID is required");

        RuleFor(x => x.BookingDate)
            .NotEmpty()
            .WithMessage("Booking date is required")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Booking date cannot be in the past");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required")
            .LessThan(x => x.EndTime)
            .WithMessage("Start time must be before end time");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required");

        RuleFor(x => x.Purpose)
            .NotEmpty()
            .WithMessage("Purpose is required")
            .MaximumLength(500)
            .WithMessage("Purpose must not exceed 500 characters");

        RuleFor(x => x.NumParticipants)
            .GreaterThan(0)
            .WithMessage("Number of participants must be greater than 0");

        RuleFor(x => x.EquipmentNeeded)
            .MaximumLength(500)
            .WithMessage("Equipment needed must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.EquipmentNeeded));

        RuleFor(x => x.Note)
            .MaximumLength(1000)
            .WithMessage("Note must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Note));

        RuleFor(x => x.LecturerEmail)
            .EmailAddress()
            .WithMessage("Invalid email format")
            .Must(email => email == null || email.EndsWith("@fpt.edu.vn"))
            .WithMessage("Lecturer email must be an @fpt.edu.vn address")
            .When(x => !string.IsNullOrEmpty(x.LecturerEmail));
    }
}
