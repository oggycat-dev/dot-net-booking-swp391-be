using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetCampusById;

public class GetCampusByIdQueryHandler : IRequestHandler<GetCampusByIdQuery, CampusDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCampusByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CampusDto?> Handle(GetCampusByIdQuery request, CancellationToken cancellationToken)
    {
        var campus = await _unitOfWork.Campuses.GetByIdAsync(request.Id);
        
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
