using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using CleanArchitectureTemplate.Domain.Commons;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.ApproveCampusChange;

public class ApproveCampusChangeCommandHandler : IRequestHandler<ApproveCampusChangeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public ApproveCampusChangeCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ApproveCampusChangeCommand request, CancellationToken cancellationToken)
    {
        // Get current admin user
        var adminId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var admin = await _unitOfWork.Users.GetByIdAsync(adminId)
            ?? throw new NotFoundException(nameof(User), adminId);

        // Verify admin role
        if (admin.Role != UserRole.Admin)
        {
            throw new ValidationException("Only admin users can approve campus change requests");
        }

        // Get campus change request
        var campusChangeRequest = await _unitOfWork.CampusChangeRequests.GetByIdAsync(request.RequestId)
            ?? throw new NotFoundException(nameof(CampusChangeRequest), request.RequestId);

        // Check if request is still pending
        if (!campusChangeRequest.IsPending())
        {
            throw new ValidationException($"This request has already been {campusChangeRequest.Status.ToString().ToLower()}");
        }

        // Get user who made the request
        var user = await _unitOfWork.Users.GetByIdAsync(campusChangeRequest.UserId)
            ?? throw new NotFoundException(nameof(User), campusChangeRequest.UserId);

        if (request.Approved)
        {
            // Approve and change user's campus
            campusChangeRequest.Approve(adminId, request.Comment);
            
            // Update user's campus
            user.CampusId = campusChangeRequest.RequestedCampusId;
            user.MarkAsModified();
            
            await _unitOfWork.Users.UpdateAsync(user);
            
            // Send email notification to user about approval
            var newCampus = await _unitOfWork.Campuses.GetByIdAsync(campusChangeRequest.RequestedCampusId);
            try
            {
                await _emailService.SendCampusChangeApprovedEmailAsync(user.Email, user.FullName, newCampus?.CampusName ?? "New Campus");
            }
            catch (Exception ex)
            {
                // Log error but don't fail the operation
            }
        }
        else
        {
            // Reject the request
            var rejectionReason = request.Comment ?? "No reason provided";
            campusChangeRequest.Reject(adminId, rejectionReason);
            
            // Send email notification to user about rejection
            try
            {
                await _emailService.SendCampusChangeRejectedEmailAsync(user.Email, user.FullName, rejectionReason);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the operation
            }
        }

        await _unitOfWork.CampusChangeRequests.UpdateAsync(campusChangeRequest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
