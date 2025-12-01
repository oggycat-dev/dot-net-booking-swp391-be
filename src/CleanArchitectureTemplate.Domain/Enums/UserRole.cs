namespace CleanArchitectureTemplate.Domain.Enums;

/// <summary>
/// User roles enumeration
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular user
    /// </summary>
    User = 0,
    
    /// <summary>
    /// Administrator user
    /// </summary>
    Admin = 1,
    
    /// <summary>
    /// Moderator user
    /// </summary>
    Moderator = 2
}

/// <summary>
/// Status enumeration for general use
/// </summary>
public enum Status
{
    /// <summary>
    /// Inactive status
    /// </summary>
    Inactive = 0,
    
    /// <summary>
    /// Active status
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Pending status
    /// </summary>
    Pending = 2,
    
    /// <summary>
    /// Suspended status
    /// </summary>
    Suspended = 3
}
