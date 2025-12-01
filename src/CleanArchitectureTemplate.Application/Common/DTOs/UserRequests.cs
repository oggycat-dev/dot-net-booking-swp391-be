namespace CleanArchitectureTemplate.Application.Common.DTOs;

/// <summary>
/// Create user request DTO
/// </summary>
public class CreateUserRequest
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
}

/// <summary>
/// Update user request DTO
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; set; }
    
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
    /// Whether user is active
    /// </summary>
    public bool IsActive { get; set; }
}
