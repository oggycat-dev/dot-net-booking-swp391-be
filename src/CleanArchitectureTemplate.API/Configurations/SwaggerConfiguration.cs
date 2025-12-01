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
            // API Document (Public/User APIs)
            c.SwaggerDoc("api", new OpenApiInfo
            {
                Title = "Clean Architecture Template - User API",
                Version = "v1",
                Description = "Public APIs for user authentication and general operations",
                Contact = new OpenApiContact
                {
                    Name = "Developer",
                    Email = "developer@example.com"
                }
            });

            // CMS Document (Admin APIs)
            c.SwaggerDoc("cms", new OpenApiInfo
            {
                Title = "Clean Architecture Template - CMS API",
                Version = "v1",
                Description = "Admin APIs for content management system (CMS)",
                Contact = new OpenApiContact
                {
                    Name = "Developer",
                    Email = "developer@example.com"
                }
            });

            // Group APIs by path
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (string.IsNullOrEmpty(apiDesc.RelativePath))
                    return false;

                if (docName == "api")
                {
                    // Include only /api/Auth and other public APIs
                    return apiDesc.RelativePath.StartsWith("api/Auth", StringComparison.OrdinalIgnoreCase) ||
                           (apiDesc.RelativePath.StartsWith("api/", StringComparison.OrdinalIgnoreCase) &&
                            !apiDesc.RelativePath.StartsWith("api/cms/", StringComparison.OrdinalIgnoreCase));
                }

                if (docName == "cms")
                {
                    // Include only /api/cms/* APIs
                    return apiDesc.RelativePath.StartsWith("api/cms/", StringComparison.OrdinalIgnoreCase);
                }

                return false;
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
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
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
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // User API endpoint
                c.SwaggerEndpoint("/swagger/api/swagger.json", "User API v1");
                
                // CMS API endpoint
                c.SwaggerEndpoint("/swagger/cms/swagger.json", "CMS API v1");
                
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                c.DisplayRequestDuration(); // Show request duration
            });
        }

        return app;
    }
}
