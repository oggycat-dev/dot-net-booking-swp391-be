namespace CleanArchitectureTemplate.Application.Common.DTOs.Auth;

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string Token, 
    string RefreshToken,
    string Role);

public record RegisterRequest(string Username, string Email, string Password, string FirstName, string LastName);

public record RefreshTokenRequest(string RefreshToken);