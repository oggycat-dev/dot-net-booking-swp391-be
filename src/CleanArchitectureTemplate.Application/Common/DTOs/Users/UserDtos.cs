using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Application.Common.DTOs.Users;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required UserRole Role { get; init; }
    public required bool IsActive { get; init; }
    public DateTime? LastLoginDate { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateUserRequest
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required UserRole Role { get; init; }
}

public record UpdateUserRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
    public required bool IsActive { get; init; }
}