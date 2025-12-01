using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetCampusById;

public record GetCampusByIdQuery(Guid Id) : IRequest<CampusDto?>;
