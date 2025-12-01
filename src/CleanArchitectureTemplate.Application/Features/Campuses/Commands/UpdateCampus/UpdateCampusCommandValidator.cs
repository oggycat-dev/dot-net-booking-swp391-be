using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Commands.UpdateCampus;

public class UpdateCampusCommandValidator : AbstractValidator<UpdateCampusCommand>
{
    public UpdateCampusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Campus ID is required");

        RuleFor(x => x.CampusName)
            .NotEmpty().WithMessage("Campus name is required")
            .MaximumLength(200).WithMessage("Campus name cannot exceed 200 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.ContactPhone)
            .MaximumLength(20).WithMessage("Contact phone cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.ContactPhone));

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Contact email cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));
    }
}
