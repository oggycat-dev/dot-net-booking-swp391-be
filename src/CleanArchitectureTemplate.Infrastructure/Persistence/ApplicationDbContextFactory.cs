using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CleanArchitectureTemplate.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for Entity Framework migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Load configuration from .env file and appsettings
        var projectRoot = Directory.GetCurrentDirectory();
        var envPath = Path.Combine(projectRoot, "..", "..", ".env");
        if (File.Exists(envPath))
        {
            DotNetEnv.Env.Load(envPath);
        }
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectRoot)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=CleanArchitectureTemplateDb;Username=postgres;Password=12345";
            
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options, new MockCurrentUserService());
    }
}

/// <summary>
/// Mock current user service for design-time operations
/// </summary>
internal class MockCurrentUserService : Application.Common.Interfaces.ICurrentUserService
{
    public Guid? UserId => Guid.NewGuid();
    public string? UserEmail => "system@example.com";
    public bool IsAuthenticated => true;
}
