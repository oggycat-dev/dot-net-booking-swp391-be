using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CleanArchitectureTemplate.API.Configurations;

/// <summary>
/// Swagger configuration
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Add Swagger services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Single API Document
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Facility Booking System API",
                Version = "v1",
                Description = "APIs for facility booking management system",
                Contact = new OpenApiContact
                {
                    Name = "Developer",
                    Email = "developer@example.com"
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Add JWT authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Use Swagger middleware
    /// </summary>
    /// <param name="app">Web application</param>
    /// <param name="environment">Environment</param>
    /// <returns>Web application</returns>
    public static WebApplication UseSwaggerConfiguration(this WebApplication app, IWebHostEnvironment environment)
    {
        // Enable Swagger for both Development and Production
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Facility Booking System API v1");
            c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
            c.DisplayRequestDuration(); // Show request duration
        });

        return app;
    }
}
