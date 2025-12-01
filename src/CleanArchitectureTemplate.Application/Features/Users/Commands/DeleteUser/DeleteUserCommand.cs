using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Unit>;