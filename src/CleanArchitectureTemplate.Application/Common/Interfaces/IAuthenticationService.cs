using CleanArchitectureTemplate.Application.Common.DTOs.Auth;

namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task LogoutAsync(string userId);
}