using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.ValueObjects;
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

        // Check if admin exists
        var adminEmail = _configuration["Admin:Email"] ?? "admin@fpt.edu.vn";
        var existingAdmin = await unitOfWork.Users.GetByEmailAsync(adminEmail);
        
        if (existingAdmin == null)
        {
            var admin = new User
            {
                UserCode = _configuration["Admin:Username"] ?? "ADMIN",
                FullName = $"{_configuration["Admin:FirstName"] ?? "System"} {_configuration["Admin:LastName"] ?? "Administrator"}",
                Email = adminEmail,
                Role = UserRole.Admin,
                Department = "IT Department",
                IsActive = true,
                EmailConfirmed = true,
                IsApproved = true,
                NoShowCount = 0,
                IsBlocked = false
            };
            
            // Hash the admin password using BCrypt
            var adminPassword = _configuration["Admin:Password"] ?? "Admin@123456";
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

            await unitOfWork.Users.AddAsync(admin);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}