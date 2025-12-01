using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Commands.UpdateCampus;

public record UpdateCampusCommand : IRequest<CampusDto>
{
    public Guid Id { get; init; }
    public string CampusName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public TimeSpan WorkingHoursStart { get; init; }
    public TimeSpan WorkingHoursEnd { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
    public bool IsActive { get; init; }
}
