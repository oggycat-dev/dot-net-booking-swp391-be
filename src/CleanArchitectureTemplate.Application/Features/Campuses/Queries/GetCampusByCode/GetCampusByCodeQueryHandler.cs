using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetCampusByCode;

public class GetCampusByCodeQueryHandler : IRequestHandler<GetCampusByCodeQuery, CampusDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCampusByCodeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CampusDto?> Handle(GetCampusByCodeQuery request, CancellationToken cancellationToken)
    {
        var campus = await _unitOfWork.Campuses.GetByCodeAsync(request.Code);
        
        if (campus == null || campus.IsDeleted)
        {
            return null;
        }

        return new CampusDto(
            campus.Id,
            campus.CampusCode,
            campus.CampusName,
            campus.Address,
            campus.WorkingHoursStart,
            campus.WorkingHoursEnd,
            campus.ContactPhone,
            campus.ContactEmail,
            campus.IsActive
        );
    }
}
