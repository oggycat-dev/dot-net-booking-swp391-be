using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.DeleteFacility;

public record DeleteFacilityCommand(Guid Id) : IRequest<bool>;
