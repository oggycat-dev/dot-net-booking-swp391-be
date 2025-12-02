using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.RequestCampusChange;

public class RequestCampusChangeCommandValidator : AbstractValidator<RequestCampusChangeCommand>
{
    public RequestCampusChangeCommandValidator()
    {
        RuleFor(x => x.RequestedCampusId)
            .NotEmpty()
            .WithMessage("Requested campus ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason must not exceed 500 characters");
    }
}
