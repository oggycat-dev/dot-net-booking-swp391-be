namespace CleanArchitectureTemplate.Application.Common.DTOs;

/// <summary>
/// User DTO for responses
/// </summary>
public class UserDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; set; }
    
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
    /// User full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// User phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Whether email is confirmed
    /// </summary>
    public bool EmailConfirmed { get; set; }
    
    /// <summary>
    /// Whether user is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// User role
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
