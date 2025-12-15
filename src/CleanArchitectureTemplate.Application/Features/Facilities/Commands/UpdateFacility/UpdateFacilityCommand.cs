using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.UpdateFacility;

public record UpdateFacilityCommand : IRequest<FacilityDto>
{
    public Guid Id { get; init; }
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
    public List<IFormFile>? Images { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
