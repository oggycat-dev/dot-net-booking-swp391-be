using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitectureTemplate.Application;
using CleanArchitectureTemplate.Infrastructure;
using CleanArchitectureTemplate.API.Configurations;
using CleanArchitectureTemplate.API.Filters;
using CleanArchitectureTemplate.API.Injection;
using CleanArchitectureTemplate.API.Extensions;
using Serilog;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

// Clear default claim type mappings
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add logging configuration
builder.AddLoggingConfiguration();

// Add environment configuration from .env
builder.AddEnvironmentConfiguration();

// Get environment information
var isProduction = builder.Environment.IsProduction();
var isDevelopment = builder.Environment.IsDevelopment();

// Get logger to log startup information
var startupLogger = LoggingConfiguration.CreateStartupLogger();

// Log environment information
startupLogger.Information($"Current Environment: {builder.Environment.EnvironmentName}");
startupLogger.Information($"IsProduction: {isProduction}");
startupLogger.Information($"IsDevelopment: {isDevelopment}");
startupLogger.Information($"ConnectionString configured: {!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("DefaultConnection"))}");

// Configure JSON serialization
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilterAttribute>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Add configurations
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCorsConfiguration(builder.Configuration);

// Add application and infrastructure services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add API services
builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddAuthorization();

// Add HTTP context accessor for current user service
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
// Always apply migrations in containerized environment
var shouldApplyMigrations = isDevelopment || builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup", false);

if (shouldApplyMigrations)
{
    try
    {
        startupLogger.Information("Initializing database...");
        await app.ApplyMigrations(startupLogger);
        startupLogger.Information("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        startupLogger.Error(ex, "An error occurred during database initialization!");
        throw;
    }
}

// Add global exception handling middleware
app.UseGlobalExceptionHandling();

// Apply Swagger configuration
app.UseSwaggerConfiguration(app.Environment);

// Add static files middleware
app.UseStaticFiles();

// HTTPS redirection disabled - Azure handles SSL termination
// if (isProduction)
// {
//     app.UseHttpsRedirection();
// }

// Handle CORS
app.UseCorsConfiguration();

// Add routing
app.UseRouting();

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

try
{
    startupLogger.Information("Starting web host");
    await app.RunAsync();
}
catch (Exception ex)
{
    startupLogger.Error(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    // Ensure all logs are written
    Log.CloseAndFlush();
}
