using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFirebaseNotificationService _firebaseNotificationService;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IFirebaseNotificationService firebaseNotificationService)
    {
        _unitOfWork = unitOfWork;
        _firebaseNotificationService = firebaseNotificationService;
    }

    public async Task<ApiResponse<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if campus exists
        var campus = await _unitOfWork.Campuses.GetByIdAsync(request.CampusId);
        if (campus == null)
        {
            throw new NotFoundException("Campus", request.CampusId);
        }

        // Check if email already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ValidationException("Email already registered");
        }

        // Generate UserCode from email (part before @)
        var userCode = request.Email.Split('@')[0].ToUpper();
        
        // Check if user code already exists (unlikely but possible)
        var users = await _unitOfWork.Users.GetAllAsync();
        if (users.Any(u => u.UserCode == userCode))
        {
            // Add random suffix if duplicate
            userCode = $"{userCode}{new Random().Next(100, 999)}";
        }

        // Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Parse role
        var role = Enum.Parse<UserRole>(request.Role);

        // Create user (pending approval)
        var user = new User
        {
            UserCode = userCode,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = passwordHash,
            Role = role,
            CampusId = request.CampusId,
            Department = request.Department,
            Major = request.Major,
            IsActive = false, // Not active until approved
            IsApproved = false, // Pending admin approval
            EmailConfirmed = false
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to all admins
        await _firebaseNotificationService.SendToAllAdminsAsync(
            "New Registration Request",
            $"{user.FullName} ({user.Email}) has submitted a {role} registration request",
            new Dictionary<string, string>
            {
                { "type", "new_registration" },
                { "userId", user.Id.ToString() },
                { "userName", user.FullName },
                { "userEmail", user.Email },
                { "userRole", role.ToString() },
                { "campusId", campus.Id.ToString() },
                { "campusName", campus.CampusName }
            });

        return ApiResponse<Guid>.Ok(user.Id, "Registration submitted successfully. Please wait for admin approval.");
    }
}
