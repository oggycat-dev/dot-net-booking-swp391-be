using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.CreateIssueReport;

public class CreateIssueReportCommandValidator : AbstractValidator<CreateIssueReportCommand>
{
    public CreateIssueReportCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("BookingId is required");

        RuleFor(x => x.IssueTitle)
            .NotEmpty().WithMessage("IssueTitle is required")
            .MaximumLength(200).WithMessage("IssueTitle must not exceed 200 characters");

        RuleFor(x => x.IssueDescription)
            .NotEmpty().WithMessage("IssueDescription is required")
            .MaximumLength(2000).WithMessage("IssueDescription must not exceed 2000 characters");

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("Severity is required")
            .Must(s => new[] { "Low", "Medium", "High", "Critical" }.Contains(s))
            .WithMessage("Severity must be one of: Low, Medium, High, Critical");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(c => new[] { "Leak", "Equipment", "Cleanliness", "Safety", "Other" }.Contains(c))
            .WithMessage("Category must be one of: Leak, Equipment, Cleanliness, Safety, Other");
    }
}
