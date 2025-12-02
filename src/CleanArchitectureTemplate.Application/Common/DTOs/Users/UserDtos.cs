namespace CleanArchitectureTemplate.Application.Common.DTOs.Users;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string UserCode { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public required string Role { get; init; }
    public string? Department { get; init; }
    public string? Major { get; init; }
    public Guid? CampusId { get; init; }
    public string? CampusName { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsApproved { get; init; }
    public required bool IsBlocked { get; init; }
    public string? BlockedReason { get; init; }
    public DateTime? BlockedUntil { get; init; }
    public required int NoShowCount { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public record CreateUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FullName { get; init; }
    public required string Role { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Department { get; init; }
    public string? Major { get; init; }
    public Guid? CampusId { get; init; }
}

public record UpdateUserRequest
{
    public required Guid Id { get; init; }
    public required string FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Department { get; init; }
    public string? Major { get; init; }
    public Guid? CampusId { get; init; }
    public required bool IsActive { get; init; }
}