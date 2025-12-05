using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Queries.GetMyIssueReports;

/// <summary>
/// Query to get current user's facility issue reports
/// </summary>
public record GetMyIssueReportsQuery : IRequest<List<FacilityIssueReportDto>>
{
}
