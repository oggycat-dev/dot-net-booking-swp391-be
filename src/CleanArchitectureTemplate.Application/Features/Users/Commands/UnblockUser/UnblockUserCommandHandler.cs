using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UnblockUser;

public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UnblockUserCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        // Get current admin user
        var adminId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var admin = await _unitOfWork.Users.GetByIdAsync(adminId)
            ?? throw new NotFoundException(nameof(User), adminId);

        // Verify admin role
        if (admin.Role != UserRole.Admin)
        {
            throw new ValidationException("Only admin users can unblock users");
        }

        // Get user to unblock
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        // Check if user is blocked
        if (!user.IsBlocked)
        {
            throw new ValidationException("User is not blocked");
        }

        // Unblock user
        user.UnblockUser();
        
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
