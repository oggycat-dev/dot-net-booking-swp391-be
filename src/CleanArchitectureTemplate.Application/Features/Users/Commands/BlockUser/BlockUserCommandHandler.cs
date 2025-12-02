using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.BlockUser;

public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BlockUserCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        // Get current admin user
        var adminId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var admin = await _unitOfWork.Users.GetByIdAsync(adminId)
            ?? throw new NotFoundException(nameof(User), adminId);

        // Verify admin role
        if (admin.Role != UserRole.Admin)
        {
            throw new ValidationException("Only admin users can block users");
        }

        // Get user to block
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        // Cannot block admin users
        if (user.Role == UserRole.Admin)
        {
            throw new ValidationException("Cannot block admin users");
        }

        // Cannot block already blocked user
        if (user.IsBlocked)
        {
            throw new ValidationException("User is already blocked");
        }

        // Block user
        user.IsBlocked = true;
        user.BlockedReason = request.Reason;
        user.BlockedUntil = request.BlockedUntil;
        
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
