using CleanArchitectureTemplate.Application.Common.DTOs.Auth;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Queries.GetPendingRegistrations;

public class GetPendingRegistrationsQueryHandler : IRequestHandler<GetPendingRegistrationsQuery, List<PendingRegistrationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingRegistrationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<PendingRegistrationDto>> Handle(GetPendingRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        
        var pendingUsers = users
            .Where(u => !u.IsApproved && !u.IsActive)
            .OrderBy(u => u.CreatedAt)
            .ToList();

        var result = new List<PendingRegistrationDto>();
        
        foreach (var user in pendingUsers)
        {
            var campus = await _unitOfWork.Campuses.GetByIdAsync(user.CampusId ?? Guid.Empty);
            
            result.Add(new PendingRegistrationDto(
                user.Id,
                user.UserCode,
                user.FullName,
                user.Email,
                user.PhoneNumber ?? string.Empty,
                user.CampusId ?? Guid.Empty,
                campus?.CampusName ?? string.Empty,
                user.Role.ToString(),
                user.Department,
                user.Major,
                user.CreatedAt
            ));
        }

        return result;
    }
}
