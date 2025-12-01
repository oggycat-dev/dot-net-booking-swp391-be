using MediatR;
using AutoMapper;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Commons;
using Microsoft.AspNetCore.Identity;
using UserDto = CleanArchitectureTemplate.Application.Common.DTOs.Users.UserDto;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Create user command handler
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user with email already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return ApiResponse<UserDto>.BadRequest("User with this email already exists");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserCode = request.Email.Split('@')[0], // Extract user code from email
            FullName = $"{request.FirstName} {request.LastName}".Trim(),
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = Enum.TryParse<UserRole>(request.Role, out var role) ? role : UserRole.Student,
            EmailConfirmed = false,
            IsActive = true,
            NoShowCount = 0,
            IsBlocked = false
        };

        // Hash password if provided
        if (!string.IsNullOrEmpty(request.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        }

        // Mark as created
        user.MarkAsCreated(_currentUserService.UserId);

        // Add to repository
        await _unitOfWork.Users.AddAsync(user);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO
        var userDto = _mapper.Map<UserDto>(user);

        return ApiResponse<UserDto>.Ok(userDto, "User created successfully");
    }
}
