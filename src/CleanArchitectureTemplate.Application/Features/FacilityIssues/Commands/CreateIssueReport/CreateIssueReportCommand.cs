using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.CreateIssueReport;

public record CreateIssueReportCommand : IRequest<FacilityIssueReportDto>
{
    public Guid BookingId { get; init; }
    public string IssueTitle { get; init; } = string.Empty;
    public string IssueDescription { get; init; } = string.Empty;
    public string Severity { get; init; } = "Medium";
    public string Category { get; init; } = string.Empty;
    public List<IFormFile>? Images { get; init; }
}
