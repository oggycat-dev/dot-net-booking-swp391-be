using CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Queries.GetAllFacilityTypes;

public class GetAllFacilityTypesQueryHandler : IRequestHandler<GetAllFacilityTypesQuery, List<FacilityTypeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllFacilityTypesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FacilityTypeDto>> Handle(GetAllFacilityTypesQuery request, CancellationToken cancellationToken)
    {
        var facilityTypes = request.ActiveOnly == true
            ? await _unitOfWork.FacilityTypes.GetActiveTypesAsync()
            : await _unitOfWork.FacilityTypes.GetAllAsync();

        return facilityTypes.Select(ft => new FacilityTypeDto(
            ft.Id,
            ft.TypeCode,
            ft.TypeName,
            ft.Description,
            ft.IsActive
        )).ToList();
    }
}
