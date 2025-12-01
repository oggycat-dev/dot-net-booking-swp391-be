using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Queries.GetAllFacilities;

public class GetAllFacilitiesQueryHandler : IRequestHandler<GetAllFacilitiesQuery, List<FacilityDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllFacilitiesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FacilityDto>> Handle(GetAllFacilitiesQuery request, CancellationToken cancellationToken)
    {
        var facilities = request.AvailableOnly == true
            ? await _unitOfWork.Facilities.GetAvailableFacilitiesAsync(request.CampusId, request.FacilityTypeId)
            : await _unitOfWork.Facilities.GetAllAsync();

        if (request.CampusId.HasValue && request.AvailableOnly != true)
        {
            facilities = await _unitOfWork.Facilities.GetByCampusIdAsync(request.CampusId.Value);
        }

        if (request.FacilityTypeId.HasValue && request.AvailableOnly != true)
        {
            facilities = await _unitOfWork.Facilities.GetByFacilityTypeIdAsync(request.FacilityTypeId.Value);
        }

        return facilities.Select(f => new FacilityDto(
            f.Id,
            f.FacilityCode,
            f.FacilityName,
            f.TypeId,
            f.Type?.TypeName ?? string.Empty,
            f.CampusId,
            f.Campus?.CampusName ?? string.Empty,
            f.Building,
            f.Floor,
            f.RoomNumber,
            f.Capacity,
            f.Description,
            f.Equipment,
            f.ImageUrl,
            f.Status.ToString(),
            f.IsActive
        )).ToList();
    }
}
