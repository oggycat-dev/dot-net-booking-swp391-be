using Microsoft.AspNetCore.Http;
using CleanArchitectureTemplate.Application.Common.Interfaces;

namespace CleanArchitectureTemplate.Infrastructure.Services;

/// <summary>
/// Current user service implementation
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ??
                             _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
