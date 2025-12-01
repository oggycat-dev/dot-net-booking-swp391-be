using CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Queries.GetAllFacilityTypes;

public record GetAllFacilityTypesQuery(bool? ActiveOnly = null) : IRequest<List<FacilityTypeDto>>;
