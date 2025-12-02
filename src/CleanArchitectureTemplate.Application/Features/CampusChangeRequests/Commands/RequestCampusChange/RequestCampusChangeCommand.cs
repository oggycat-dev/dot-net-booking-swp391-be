using CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.RequestCampusChange;

public record RequestCampusChangeCommand : IRequest<CampusChangeRequestDto>
{
    public Guid RequestedCampusId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
