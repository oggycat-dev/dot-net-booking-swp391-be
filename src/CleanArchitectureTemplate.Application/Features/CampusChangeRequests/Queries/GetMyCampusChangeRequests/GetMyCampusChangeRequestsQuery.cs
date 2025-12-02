using CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Queries.GetMyCampusChangeRequests;

public record GetMyCampusChangeRequestsQuery : IRequest<List<MyCampusChangeRequestDto>>;
