using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CleanArchitectureTemplate.Infrastructure.Persistence;
using Serilog;

namespace CleanArchitectureTemplate.API.Extensions;

/// <summary>
/// Migration extensions
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Apply database migrations
    /// </summary>
    /// <param name="app">Web application</param>
    /// <param name="logger">Logger</param>
    /// <returns>Task</returns>
    public static async Task ApplyMigrations(this WebApplication app, Serilog.ILogger logger)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            logger.Information("Applying database migrations...");
            await context.Database.MigrateAsync();
            logger.Information("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while applying database migrations");
            throw;
        }
    }
}
