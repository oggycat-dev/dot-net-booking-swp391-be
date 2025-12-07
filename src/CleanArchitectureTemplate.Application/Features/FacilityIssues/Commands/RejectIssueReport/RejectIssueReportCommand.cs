using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.RejectIssueReport;

public class RejectIssueReportCommand : IRequest<FacilityIssueReportDto>
{
    public Guid ReportId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}
