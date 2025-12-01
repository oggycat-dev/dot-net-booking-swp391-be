using CleanArchitectureTemplate.Application.Common.DTOs.Campus;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Campuses.Queries.GetAllCampuses;

public class GetAllCampusesQueryHandler : IRequestHandler<GetAllCampusesQuery, List<CampusDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCampusesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CampusDto>> Handle(GetAllCampusesQuery request, CancellationToken cancellationToken)
    {
        var campuses = await _unitOfWork.Campuses.GetActiveCampusesAsync();
        
        return campuses.Select(c => new CampusDto(
            c.Id,
            c.CampusCode,
            c.CampusName,
            c.Address,
            c.WorkingHoursStart,
            c.WorkingHoursEnd,
            c.ContactPhone,
            c.ContactEmail,
            c.IsActive
        )).ToList();
    }
}
