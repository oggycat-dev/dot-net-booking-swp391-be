using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Queries.GetFacilityById;

public class GetFacilityByIdQueryHandler : IRequestHandler<GetFacilityByIdQuery, FacilityDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFacilityByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FacilityDto?> Handle(GetFacilityByIdQuery request, CancellationToken cancellationToken)
    {
        var facility = await _unitOfWork.Facilities.GetByIdAsync(request.Id);

        if (facility == null || facility.IsDeleted)
        {
            return null;
        }

        return new FacilityDto(
            facility.Id,
            facility.FacilityCode,
            facility.FacilityName,
            facility.TypeId,
            facility.Type?.TypeName ?? string.Empty,
            facility.CampusId,
            facility.Campus?.CampusName ?? string.Empty,
            facility.Building,
            facility.Floor,
            facility.RoomNumber,
            facility.Capacity,
            facility.Description,
            facility.Equipment,
            facility.ImageUrl,
            facility.Status.ToString(),
            facility.IsActive
        );
    }
}
