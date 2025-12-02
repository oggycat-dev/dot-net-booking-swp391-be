using CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.CampusChangeRequests.Queries.GetPendingCampusChangeRequests;

public class GetPendingCampusChangeRequestsQueryHandler : IRequestHandler<GetPendingCampusChangeRequestsQuery, List<CampusChangeRequestDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingCampusChangeRequestsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CampusChangeRequestDto>> Handle(GetPendingCampusChangeRequestsQuery request, CancellationToken cancellationToken)
    {
        var pendingRequests = await _unitOfWork.CampusChangeRequests.GetPendingRequestsAsync();

        return pendingRequests.Select(r => new CampusChangeRequestDto(
            r.Id,
            r.UserId,
            r.User.FullName,
            r.User.Email,
            r.CurrentCampusId,
            r.CurrentCampus?.CampusName,
            r.RequestedCampusId,
            r.RequestedCampus.CampusName,
            r.Reason,
            r.Status.ToString(),
            r.ReviewedBy,
            r.ReviewedByAdmin?.FullName,
            r.ReviewedAt,
            r.ReviewComment,
            r.CreatedAt
        )).ToList();
    }
}
