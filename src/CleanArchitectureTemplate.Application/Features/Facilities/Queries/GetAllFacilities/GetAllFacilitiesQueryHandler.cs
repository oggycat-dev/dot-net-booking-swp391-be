using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Queries.GetAllFacilities;

public class GetAllFacilitiesQueryHandler : IRequestHandler<GetAllFacilitiesQuery, List<FacilityDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetAllFacilitiesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<List<FacilityDto>> Handle(GetAllFacilitiesQuery request, CancellationToken cancellationToken)
    {
        // Auto-filter by campus for Student/Lecturer
        var campusId = request.CampusId;
        if (!campusId.HasValue && _currentUserService.IsAuthenticated)
        {
            var role = _currentUserService.Role;
            if (role == UserRole.Student.ToString() || role == UserRole.Lecturer.ToString())
            {
                campusId = _currentUserService.CampusId;
            }
        }

        var facilities = request.AvailableOnly == true
            ? await _unitOfWork.Facilities.GetAvailableFacilitiesAsync(campusId, request.FacilityTypeId)
            : await _unitOfWork.Facilities.GetAllAsync();

        if (campusId.HasValue && request.AvailableOnly != true)
        {
            facilities = await _unitOfWork.Facilities.GetByCampusIdAsync(campusId.Value);
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
