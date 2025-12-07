using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.RejectIssueReport;

public class RejectIssueReportCommandValidator : AbstractValidator<RejectIssueReportCommand>
{
    public RejectIssueReportCommandValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty().WithMessage("Report ID is required");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required")
            .MaximumLength(1000).WithMessage("Rejection reason must not exceed 1000 characters");
    }
}
