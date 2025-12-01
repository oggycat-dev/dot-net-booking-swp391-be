using Microsoft.AspNetCore.Authorization;

namespace CleanArchitectureTemplate.API.Authorization;

/// <summary>
/// Authorization policies for role-based access control
/// </summary>
public static class Policies
{
    public const string StudentOnly = "StudentOnly";
    public const string LecturerOnly = "LecturerOnly";
    public const string AdminOnly = "AdminOnly";
    public const string LecturerOrAdmin = "LecturerOrAdmin";
    public const string AllRoles = "AllRoles";
}

/// <summary>
/// Authorization requirements for custom policies
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public string[] Roles { get; }

    public RoleRequirement(params string[] roles)
    {
        Roles = roles;
    }
}

/// <summary>
/// Handler for role-based authorization
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        var userRole = context.User.FindFirst("role")?.Value;
        if (userRole != null && requirement.Roles.Contains(userRole))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
