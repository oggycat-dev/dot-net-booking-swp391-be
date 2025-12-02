using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UnblockUser;

public record UnblockUserCommand(Guid UserId) : IRequest<Unit>;
