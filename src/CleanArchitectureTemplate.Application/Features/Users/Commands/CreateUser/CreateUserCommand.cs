using MediatR;
using CleanArchitectureTemplate.Application.Common.DTOs;
using UserDto = CleanArchitectureTemplate.Application.Common.DTOs.Users.UserDto;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Create user command following CQRS pattern
/// </summary>
public class CreateUserCommand : IRequest<ApiResponse<UserDto>>
{
    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// User phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// User role
    /// </summary>
    public string Role { get; set; } = "User";
    
    /// <summary>
    /// User password
    /// </summary>
    public string? Password { get; set; }
}
