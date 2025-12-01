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
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@fpt\.edu\.vn$", ErrorMessage = "Email must be @fpt.edu.vn domain")]
    string Email,
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
    string Password,
    
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(200, ErrorMessage = "Full name cannot exceed 200 characters")]
    string FullName,
    
    [Required(ErrorMessage = "Campus selection is required")]
    Guid CampusId,
    
    string? UserCode
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