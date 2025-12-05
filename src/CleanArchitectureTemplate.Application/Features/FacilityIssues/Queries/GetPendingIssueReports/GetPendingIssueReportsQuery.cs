using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Queries.GetPendingIssueReports;

/// <summary>
/// Query to get pending facility issue reports for admin
/// </summary>
public record GetPendingIssueReportsQuery : IRequest<List<FacilityIssueReportDto>>
{
}
