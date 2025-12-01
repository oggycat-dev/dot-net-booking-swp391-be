namespace CleanArchitectureTemplate.Application.Common.Interfaces;

/// <summary>
/// Current user service interface
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Current user ID
    /// </summary>
    Guid? UserId { get; }
    
    /// <summary>
    /// Current user email
    /// </summary>
    string? UserEmail { get; }
    
    /// <summary>
    /// Current user's campus ID
    /// </summary>
    Guid? CampusId { get; }
    
    /// <summary>
    /// Current user's role
    /// </summary>
    string? Role { get; }
    
    /// <summary>
    /// Whether current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
