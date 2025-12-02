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
        var adminPassword = _configuration["Admin:Password"] ?? "Admin@123456";
        
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
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

            await unitOfWork.Users.AddAsync(admin);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // Update admin account to ensure it's properly configured
            bool needsUpdate = false;
            
            // Always rehash password to ensure it's valid BCrypt format
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
            if (existingAdmin.PasswordHash != newPasswordHash)
            {
                existingAdmin.PasswordHash = newPasswordHash;
                needsUpdate = true;
            }
            
            // Ensure admin is approved and active
            if (!existingAdmin.IsApproved)
            {
                existingAdmin.IsApproved = true;
                needsUpdate = true;
            }
            
            if (!existingAdmin.IsActive)
            {
                existingAdmin.IsActive = true;
                needsUpdate = true;
            }
            
            if (existingAdmin.IsBlocked)
            {
                existingAdmin.IsBlocked = false;
                existingAdmin.BlockedUntil = null;
                existingAdmin.BlockedReason = null;
                needsUpdate = true;
            }
            
            if (needsUpdate)
            {
                await unitOfWork.Users.UpdateAsync(existingAdmin);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}