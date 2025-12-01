using System.ComponentModel.DataAnnotations;

namespace CleanArchitectureTemplate.Application.Common.DTOs.Auth;

public record LoginRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,
    
    [Required(ErrorMessage = "Password is required")]
    string Password
);

public record LoginResponse(
    string Token,
    string RefreshToken,
    string Role,
    UserInfoDto User
);

public record UserInfoDto(
    Guid Id,
    string UserCode,
    string FullName,
    string Email,
    string Role,
    Guid? CampusId,
    string? CampusName
);

public record RegisterRequest(
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    string FullName,
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@fpt\.edu\.vn$", ErrorMessage = "Email must be @fpt.edu.vn domain")]
    string Email,
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
    string Password,
    
    [Required(ErrorMessage = "Confirm password is required")]
    string ConfirmPassword,
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Phone number must be 10-11 digits")]
    string PhoneNumber,
    
    [Required(ErrorMessage = "Campus selection is required")]
    Guid CampusId,
    
    [Required(ErrorMessage = "Role is required")]
    [RegularExpression(@"^(Student|Lecturer)$", ErrorMessage = "Role must be either Student or Lecturer")]
    string Role,
    
    string? Department,
    
    string? Major
);

/// <summary>
/// Pending registration DTO for admin review
/// </summary>
public record PendingRegistrationDto(
    Guid Id,
    string UserCode,
    string FullName,
    string Email,
    string PhoneNumber,
    Guid CampusId,
    string CampusName,
    string Role,
    string? Department,
    string? Major,
    DateTime CreatedAt
);

/// <summary>
/// Approve registration request
/// </summary>
public record ApproveRegistrationRequest(
    Guid UserId,
    bool IsApproved,
    string? RejectionReason
);

public record RefreshTokenRequest(
    [Required(ErrorMessage = "Refresh token is required")]
    string RefreshToken
);

public record ChangePasswordRequest(
    [Required(ErrorMessage = "Current password is required")]
    string CurrentPassword,
    
    [Required(ErrorMessage = "New password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
    string NewPassword
);