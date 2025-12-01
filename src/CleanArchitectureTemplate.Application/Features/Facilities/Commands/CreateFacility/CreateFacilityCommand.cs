using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.CreateFacility;

public record CreateFacilityCommand : IRequest<FacilityDto>
{
    public string FacilityCode { get; init; } = string.Empty;
    public string FacilityName { get; init; } = string.Empty;
    public Guid TypeId { get; init; }
    public Guid CampusId { get; init; }
    public string? Building { get; init; }
    public string? Floor { get; init; }
    public string? RoomNumber { get; init; }
    public int Capacity { get; init; }
    public string? Description { get; init; }
    public string? Equipment { get; init; }
    public string? ImageUrl { get; init; }
}
