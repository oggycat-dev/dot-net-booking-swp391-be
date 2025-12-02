using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.ApproveCampusChange;

public class ApproveCampusChangeCommandValidator : AbstractValidator<ApproveCampusChangeCommand>
{
    public ApproveCampusChangeCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("Request ID is required");

        RuleFor(x => x.Comment)
            .MaximumLength(500)
            .WithMessage("Comment must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}
