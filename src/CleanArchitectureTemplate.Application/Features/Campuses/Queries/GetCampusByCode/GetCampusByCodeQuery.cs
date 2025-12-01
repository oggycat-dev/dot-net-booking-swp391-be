using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetCampusByCode;

public record GetCampusByCodeQuery(string Code) : IRequest<CampusDto?>;
