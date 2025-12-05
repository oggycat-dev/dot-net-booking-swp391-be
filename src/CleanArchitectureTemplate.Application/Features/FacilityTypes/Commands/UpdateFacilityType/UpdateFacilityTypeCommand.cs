using CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.UpdateFacilityType;

public record UpdateFacilityTypeCommand : IRequest<FacilityTypeDto>
{
    public Guid Id { get; init; }
    public string TypeName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}
