using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.ChangeRoomForIssue;

public class ChangeRoomForIssueCommandValidator : AbstractValidator<ChangeRoomForIssueCommand>
{
    public ChangeRoomForIssueCommandValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("ReportId is required");

        RuleFor(x => x.NewFacilityId)
            .NotEmpty().WithMessage("NewFacilityId is required");

        RuleFor(x => x.AdminResponse)
            .NotEmpty().WithMessage("AdminResponse is required")
            .MaximumLength(1000).WithMessage("AdminResponse must not exceed 1000 characters");
    }
}
