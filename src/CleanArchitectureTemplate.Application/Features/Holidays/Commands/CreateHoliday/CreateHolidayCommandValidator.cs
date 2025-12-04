using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Holidays.Commands.CreateHoliday;

public class CreateHolidayCommandValidator : AbstractValidator<CreateHolidayCommand>
{
    public CreateHolidayCommandValidator()
    {
        RuleFor(x => x.HolidayName)
            .NotEmpty().WithMessage("Holiday name is required")
            .MaximumLength(200).WithMessage("Holiday name cannot exceed 200 characters");

        RuleFor(x => x.HolidayDate)
            .NotEmpty().WithMessage("Holiday date is required");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }
}
