using CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Queries.GetPendingCampusChangeRequests;

public record GetPendingCampusChangeRequestsQuery : IRequest<List<CampusChangeRequestDto>>;
