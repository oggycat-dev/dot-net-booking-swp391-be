using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Example User entity following DDD principles
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Whether the user's email is confirmed
    /// </summary>
    public bool EmailConfirmed { get; set; }
    
    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// User's role
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;
    
    /// <summary>
    /// User's hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name computed property
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Domain method to activate user account
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Domain method to deactivate user account
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Domain method to confirm email
    /// </summary>
    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        this.MarkAsModified();
    }
}
