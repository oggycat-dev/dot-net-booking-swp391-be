using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Phone number is invalid")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Department));

        RuleFor(x => x.Major)
            .MaximumLength(100).WithMessage("Major cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Major));
    }
}
