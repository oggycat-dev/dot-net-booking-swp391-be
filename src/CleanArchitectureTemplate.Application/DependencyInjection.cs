using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace CleanArchitectureTemplate.Application;

/// <summary>
/// Application layer dependency injection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add application services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        // Add AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // Add FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
