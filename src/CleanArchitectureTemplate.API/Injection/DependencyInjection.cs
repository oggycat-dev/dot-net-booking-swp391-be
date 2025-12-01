using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureTemplate.API.Middlewares;
using CleanArchitectureTemplate.API.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace CleanArchitectureTemplate.API.Injection;

/// <summary>
/// API services dependency injection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add API services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var secretKey = configuration["JWT:SecretKey"] 
                ?? throw new InvalidOperationException("JWT:SecretKey is not configured");
            var issuer = configuration["JWT:Issuer"] 
                ?? throw new InvalidOperationException("JWT:Issuer is not configured");
            var audience = configuration["JWT:Audience"] 
                ?? throw new InvalidOperationException("JWT:Audience is not configured");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                RoleClaimType = ClaimTypes.Role,  // Use ClaimTypes.Role to match auto-mapped claim type
                NameClaimType = ClaimTypes.Name
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                    logger.LogInformation("Token validated. Claims: {Claims}", string.Join(", ", claims ?? Array.Empty<string>()));
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(context.Exception, "Authentication failed");
                    return Task.CompletedTask;
                }
            };
        });

        // Add Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.StudentOnly, policy =>
                policy.RequireRole("Student"));
            
            options.AddPolicy(Policies.LecturerOnly, policy =>
                policy.RequireRole("Lecturer"));
            
            options.AddPolicy(Policies.AdminOnly, policy =>
                policy.RequireRole("Admin"));
            
            options.AddPolicy(Policies.LecturerOrAdmin, policy =>
                policy.RequireRole("Lecturer", "Admin"));
            
            options.AddPolicy(Policies.AllRoles, policy =>
                policy.RequireRole("Student", "Lecturer", "Admin"));
        });

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
        
        return services;
    }

    /// <summary>
    /// Use global exception handling middleware
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>Web application</returns>
    public static WebApplication UseGlobalExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        return app;
    }
}
