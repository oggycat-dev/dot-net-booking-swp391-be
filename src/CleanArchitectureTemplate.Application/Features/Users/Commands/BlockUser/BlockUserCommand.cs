using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.BlockUser;

public record BlockUserCommand(
    Guid UserId,
    string Reason,
    DateTime? BlockedUntil = null
) : IRequest<Unit>;
