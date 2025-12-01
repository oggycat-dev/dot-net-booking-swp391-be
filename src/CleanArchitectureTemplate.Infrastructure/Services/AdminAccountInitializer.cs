using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public class AdminAccountInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public AdminAccountInitializer(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        // Check if admin exists
        var adminEmail = _configuration["Admin:Email"] ?? "admin@example.com";
        var existingAdmin = await unitOfWork.Users.GetByEmailAsync(adminEmail);
        
        if (existingAdmin == null)
        {
            var admin = new User
            {
                Email = adminEmail,
                FirstName = _configuration["Admin:FirstName"] ?? "Admin",
                LastName = _configuration["Admin:LastName"] ?? "User",
                Role = UserRole.Admin,
                IsActive = true,
                EmailConfirmed = true
            };
            
            // Hash the admin password
            var adminPassword = _configuration["Admin:Password"] ?? "Admin@123456";
            admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword);

            await unitOfWork.Users.AddAsync(admin);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}