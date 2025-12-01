using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CleanArchitectureTemplate.Application.Common.DTOs.Auth;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username)
            ?? throw new UnauthorizedAccessException("Invalid username or password");

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        await _unitOfWork.SaveChangesAsync();

        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        return new LoginResponse(token, refreshToken, user.Role.ToString());
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var userId = _tokenService.ValidateRefreshToken(request.RefreshToken);
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        return new LoginResponse(token, refreshToken, user.Role.ToString());
    }

    public async Task LogoutAsync(string userId)
    {
        // In a real application, you might want to invalidate the refresh token
        // This would require storing refresh tokens in a database
        await Task.CompletedTask;
    }
}