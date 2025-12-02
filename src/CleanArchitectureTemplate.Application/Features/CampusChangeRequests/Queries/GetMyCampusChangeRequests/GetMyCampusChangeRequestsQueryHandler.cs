using CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Queries.GetMyCampusChangeRequests;

public class GetMyCampusChangeRequestsQueryHandler : IRequestHandler<GetMyCampusChangeRequestsQuery, List<MyCampusChangeRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyCampusChangeRequestsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<List<MyCampusChangeRequestDto>> Handle(GetMyCampusChangeRequestsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var myRequests = await _unitOfWork.CampusChangeRequests.GetByUserIdAsync(userId);

        return myRequests.Select(r => new MyCampusChangeRequestDto(
            r.Id,
            r.CurrentCampusId,
            r.CurrentCampus?.CampusName,
            r.RequestedCampusId,
            r.RequestedCampus.CampusName,
            r.Reason,
            r.Status.ToString(),
            r.ReviewComment,
            r.CreatedAt,
            r.ReviewedAt
        )).ToList();
    }
}
