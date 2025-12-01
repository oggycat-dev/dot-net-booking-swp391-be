using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Commands.DeleteCampus;

public record DeleteCampusCommand(Guid Id) : IRequest<Unit>;
