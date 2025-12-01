using MediatR;
using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.Models;
using UserDto = CleanArchitectureTemplate.Application.Common.DTOs.Users.UserDto;

namespace CleanArchitectureTemplate.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Get users query following CQRS pattern
/// </summary>
public class GetUsersQuery : PaginatedQuery<UserDto>
{
    /// <summary>
    /// Filter by role
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }
}
