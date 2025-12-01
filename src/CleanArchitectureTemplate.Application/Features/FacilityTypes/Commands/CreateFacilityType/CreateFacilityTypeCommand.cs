using CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.CreateFacilityType;

public record CreateFacilityTypeCommand : IRequest<FacilityTypeDto>
{
    public string TypeCode { get; init; } = string.Empty;
    public string TypeName { get; init; } = string.Empty;
    public string? Description { get; init; }
}
