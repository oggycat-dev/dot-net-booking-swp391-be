using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ApproveRegistration;

public class ApproveRegistrationCommandHandler : IRequestHandler<ApproveRegistrationCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public ApproveRegistrationCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<ApiResponse<bool>> Handle(ApproveRegistrationCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        if (user.IsApproved)
        {
            throw new ValidationException("User registration already processed");
        }

        if (request.IsApproved)
        {
            // Approve registration
            user.IsApproved = true;
            user.IsActive = true;
            user.EmailConfirmed = true;
            user.ApprovedBy = _currentUserService.UserId;
            user.ApprovedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send approval email to user
            try
            {
                await _emailService.SendRegistrationApprovedEmailAsync(user.Email, user.FullName);
            }
            catch
            {
                // Log error but don't fail the operation
                // Email failure should not prevent approval
            }

            return ApiResponse<bool>.Ok(true, "Registration approved successfully. Approval email sent to user.");
        }
        else
        {
            // Send rejection email before deleting user
            try
            {
                await _emailService.SendRegistrationRejectedEmailAsync(user.Email, user.FullName, request.RejectionReason ?? "No reason provided");
            }
            catch
            {
                // Log error but continue with deletion
            }

            // Reject registration - delete the user
            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Registration rejected. Rejection email sent to user.");
        }
    }
}
