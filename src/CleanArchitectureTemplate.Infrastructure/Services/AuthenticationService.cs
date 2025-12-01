using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using CleanArchitectureTemplate.Application.Common.DTOs.Auth;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _dbContext;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ITokenService tokenService,
        IApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Validate email domain
        if (!IsValidFptEmail(request.Email))
        {
            throw new ValidationException("Email must be @fpt.edu.vn domain");
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password");

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is disabled");
        }

        // Check if user is blocked
        if (user.IsCurrentlyBlocked())
        {
            throw new UnauthorizedAccessException($"Account is blocked until {user.BlockedUntil:yyyy-MM-dd HH:mm}. Reason: {user.BlockedReason}");
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Update last login
        user.UpdateLastLogin();
        await _unitOfWork.SaveChangesAsync();

        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        var userInfo = new UserInfoDto(
            user.Id,
            user.UserCode,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.CampusId,
            user.Campus?.CampusName
        );

        return new LoginResponse(token, refreshToken, user.Role.ToString(), userInfo);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        // Validate email domain
        if (!IsValidFptEmail(request.Email))
        {
            throw new ValidationException("Email must be @fpt.edu.vn domain");
        }

        // Check if email already exists
        if (await _unitOfWork.Users.IsEmailExistsAsync(request.Email))
        {
            throw new ValidationException("Email already registered");
        }

        // Validate campus exists
        var campus = await _unitOfWork.Campuses.GetByIdAsync(request.CampusId)
            ?? throw new NotFoundException(nameof(Campus), request.CampusId);

        if (!campus.IsActive)
        {
            throw new ValidationException("Selected campus is not active");
        }

        // Generate user code if not provided
        var userCode = request.UserCode ?? await GenerateUserCodeAsync();

        // Check if user code already exists
        if (await _unitOfWork.Users.IsUserCodeExistsAsync(userCode))
        {
            throw new ValidationException("User code already exists");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserCode = userCode,
            FullName = request.FullName,
            Email = request.Email,
            Role = UserRole.Student, // Default role is Student
            CampusId = request.CampusId,
            IsActive = true,
            NoShowCount = 0,
            IsBlocked = false,
            CreatedAt = DateTime.UtcNow
        };

        // Hash password
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Reload user with campus
        user = await _unitOfWork.Users.GetByIdAsync(user.Id) ?? user;

        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        var userInfo = new UserInfoDto(
            user.Id,
            user.UserCode,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.CampusId,
            user.Campus?.CampusName
        );

        return new LoginResponse(token, refreshToken, user.Role.ToString(), userInfo);
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

        if (user.IsCurrentlyBlocked())
        {
            throw new UnauthorizedAccessException($"Account is blocked until {user.BlockedUntil:yyyy-MM-dd HH:mm}");
        }

        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        var userInfo = new UserInfoDto(
            user.Id,
            user.UserCode,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.CampusId,
            user.Campus?.CampusName
        );

        return new LoginResponse(token, refreshToken, user.Role.ToString(), userInfo);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        // Verify current password
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.CurrentPassword);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new ValidationException("Current password is incorrect");
        }

        // Hash and update new password
        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task LogoutAsync(string userId)
    {
        // In a real application, you might want to invalidate the refresh token
        // This would require storing refresh tokens in a database
        await Task.CompletedTask;
    }

    private bool IsValidFptEmail(string email)
    {
        var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@fpt\.edu\.vn$", RegexOptions.IgnoreCase);
        return regex.IsMatch(email);
    }

    private async Task<string> GenerateUserCodeAsync()
    {
        var year = DateTime.UtcNow.Year.ToString();
        var random = new Random();
        
        for (int i = 0; i < 10; i++) // Try 10 times to generate unique code
        {
            var randomNumber = random.Next(10000, 99999);
            var userCode = $"USR{year}{randomNumber}";
            
            if (!await _unitOfWork.Users.IsUserCodeExistsAsync(userCode))
            {
                return userCode;
            }
        }

        throw new InvalidOperationException("Failed to generate unique user code");
    }
}