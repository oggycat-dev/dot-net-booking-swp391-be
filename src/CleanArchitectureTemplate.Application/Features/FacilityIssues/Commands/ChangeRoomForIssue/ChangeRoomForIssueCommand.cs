using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Commands.ChangeRoomForIssue;

public record ChangeRoomForIssueCommand : IRequest<FacilityIssueReportDto>
{
    public Guid ReportId { get; init; }
    public Guid NewFacilityId { get; init; }
    public string AdminResponse { get; init; } = string.Empty;
}
