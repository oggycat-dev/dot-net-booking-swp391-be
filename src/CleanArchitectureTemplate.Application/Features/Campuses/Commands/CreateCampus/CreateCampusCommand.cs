using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Commands.CreateCampus;

public record CreateCampusCommand : IRequest<CampusDto>
{
    public string CampusCode { get; init; } = string.Empty;
    public string CampusName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public TimeSpan WorkingHoursStart { get; init; }
    public TimeSpan WorkingHoursEnd { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
}
