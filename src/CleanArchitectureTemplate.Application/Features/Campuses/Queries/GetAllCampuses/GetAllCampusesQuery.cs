using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetAllCampuses;

public record GetAllCampusesQuery : IRequest<List<CampusDto>>;
