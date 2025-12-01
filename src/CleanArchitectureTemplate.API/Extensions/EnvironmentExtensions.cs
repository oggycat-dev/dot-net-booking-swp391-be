using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotNetEnv;

namespace CleanArchitectureTemplate.API.Extensions;

/// <summary>
/// Environment configuration extensions
/// </summary>
public static class EnvironmentExtensions
{
    /// <summary>
    /// Add environment configuration from .env file
    /// </summary>
    /// <param name="builder">Web application builder</param>
    /// <returns>Web application builder</returns>
    public static WebApplicationBuilder AddEnvironmentConfiguration(this WebApplicationBuilder builder)
    {
        // Try to find .env file in multiple locations
        var possiblePaths = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), ".env"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env"),
            Path.Combine(builder.Environment.ContentRootPath, ".env"),
            Path.Combine(builder.Environment.ContentRootPath, "..", "..", ".env")
        };

        string? foundEnvPath = null;
        foreach (var path in possiblePaths)
        {
            var normalizedPath = Path.GetFullPath(path);
            if (File.Exists(normalizedPath))
            {
                foundEnvPath = normalizedPath;
                break;
            }
        }

        // Load .env file if found
        if (foundEnvPath != null)
        {
            Env.Load(foundEnvPath);
            Console.WriteLine($"[ENV] Loaded .env file from: {foundEnvPath}");
        }
        else
        {
            Console.WriteLine("[ENV] No .env file found, using appsettings only");
        }

        // Clear existing configuration sources and rebuild with correct priority
        builder.Configuration.Sources.Clear();
        
        // 1. Base configuration from appsettings.json (lowest priority)
        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        
        // 2. Environment variables (highest priority - overrides appsettings)
        builder.Configuration.AddEnvironmentVariables();

        return builder;
    }
}
