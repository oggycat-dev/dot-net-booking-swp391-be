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

    public ApproveCampusChangeCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
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
            
            // TODO: Send email notification to user about approval
            // await _emailService.SendCampusChangeApprovedEmail(user.Email, campusChangeRequest);
        }
        else
        {
            // Reject the request
            var rejectionReason = request.Comment ?? "No reason provided";
            campusChangeRequest.Reject(adminId, rejectionReason);
            
            // TODO: Send email notification to user about rejection
            // await _emailService.SendCampusChangeRejectedEmail(user.Email, campusChangeRequest);
        }

        await _unitOfWork.CampusChangeRequests.UpdateAsync(campusChangeRequest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
