using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Queries.GetAllFacilities;

public record GetAllFacilitiesQuery(Guid? CampusId = null, Guid? FacilityTypeId = null, bool? AvailableOnly = null) : IRequest<List<FacilityDto>>;
