using CleanArchitectureTemplate.Domain.Entities;

namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken(User user);
    Guid ValidateRefreshToken(string refreshToken);
}
