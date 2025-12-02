using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.ApproveCampusChange;

public record ApproveCampusChangeCommand : IRequest<Unit>
{
    public Guid RequestId { get; init; }
    public bool Approved { get; init; }
    public string? Comment { get; init; }
}
