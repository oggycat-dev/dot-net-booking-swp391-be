using CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Commands.RequestCampusChange;

public class RequestCampusChangeCommandHandler : IRequestHandler<RequestCampusChangeCommand, CampusChangeRequestDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFirebaseNotificationService _firebaseNotificationService;

    public RequestCampusChangeCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFirebaseNotificationService firebaseNotificationService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _firebaseNotificationService = firebaseNotificationService;
    }

    public async Task<CampusChangeRequestDto> Handle(RequestCampusChangeCommand request, CancellationToken cancellationToken)
    {
        // Get current user
        var userId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        // Only Student and Lecturer can request campus change
        if (user.Role == UserRole.Admin)
        {
            throw new ValidationException("Admin users cannot request campus change");
        }

        // Check if user already has pending request
        if (await _unitOfWork.CampusChangeRequests.HasPendingRequestAsync(userId))
        {
            throw new ValidationException("You already have a pending campus change request");
        }

        // Validate requested campus exists and is active
        var requestedCampus = await _unitOfWork.Campuses.GetByIdAsync(request.RequestedCampusId)
            ?? throw new NotFoundException(nameof(Campus), request.RequestedCampusId);

        if (!requestedCampus.IsActive)
        {
            throw new ValidationException("The requested campus is not active");
        }

        // Check if user is not already in requested campus
        if (user.CampusId == request.RequestedCampusId)
        {
            throw new ValidationException("You are already in the requested campus");
        }

        // Get current campus info
        Campus? currentCampus = null;
        if (user.CampusId.HasValue)
        {
            currentCampus = await _unitOfWork.Campuses.GetByIdAsync(user.CampusId.Value);
        }

        // Create campus change request
        var campusChangeRequest = new CampusChangeRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CurrentCampusId = user.CampusId,
            RequestedCampusId = request.RequestedCampusId,
            Reason = request.Reason,
            Status = CampusChangeRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.CampusChangeRequests.AddAsync(campusChangeRequest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send notification to all admins
        var currentCampusName = currentCampus?.CampusName ?? "No Campus";
        await _firebaseNotificationService.SendToAllAdminsAsync(
            "New Campus Change Request",
            $"{user.FullName} requested to change campus from {currentCampusName} to {requestedCampus.CampusName}",
            new Dictionary<string, string>
            {
                { "type", "campus_change_request" },
                { "requestId", campusChangeRequest.Id.ToString() },
                { "userId", user.Id.ToString() },
                { "userName", user.FullName },
                { "userEmail", user.Email },
                { "currentCampusId", currentCampus?.Id.ToString() ?? "" },
                { "currentCampusName", currentCampusName },
                { "requestedCampusId", requestedCampus.Id.ToString() },
                { "requestedCampusName", requestedCampus.CampusName }
            });

        return new CampusChangeRequestDto(
            campusChangeRequest.Id,
            user.Id,
            user.FullName,
            user.Email,
            currentCampus?.Id,
            currentCampus?.CampusName,
            requestedCampus.Id,
            requestedCampus.CampusName,
            campusChangeRequest.Reason,
            campusChangeRequest.Status.ToString(),
            null,
            null,
            null,
            null,
            campusChangeRequest.CreatedAt
        );
    }
}
