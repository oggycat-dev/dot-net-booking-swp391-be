using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest<UserDto>
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
    public required bool IsActive { get; init; }
}