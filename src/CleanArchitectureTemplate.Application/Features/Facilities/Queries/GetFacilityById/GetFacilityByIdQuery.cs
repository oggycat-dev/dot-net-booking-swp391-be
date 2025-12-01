using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Queries.GetFacilityById;

public record GetFacilityByIdQuery(Guid Id) : IRequest<FacilityDto?>;
