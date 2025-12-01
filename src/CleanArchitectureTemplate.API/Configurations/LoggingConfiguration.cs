using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole;

namespace CleanArchitectureTemplate.API.Configurations;

/// <summary>
/// Logging configuration
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Add logging configuration
    /// </summary>
    /// <param name="builder">Web application builder</param>
    /// <returns>Web application builder</returns>
    public static WebApplicationBuilder AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: Serilog.RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    /// <summary>
    /// Create startup logger
    /// </summary>
    /// <returns>Logger instance</returns>
    public static Serilog.ILogger CreateStartupLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
    }
}
