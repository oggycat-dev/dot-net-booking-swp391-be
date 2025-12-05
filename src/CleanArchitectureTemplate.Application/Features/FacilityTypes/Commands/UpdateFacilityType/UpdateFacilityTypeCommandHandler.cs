using CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.UpdateFacilityType;

public class UpdateFacilityTypeCommandHandler : IRequestHandler<UpdateFacilityTypeCommand, FacilityTypeDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFacilityTypeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FacilityTypeDto> Handle(UpdateFacilityTypeCommand request, CancellationToken cancellationToken)
    {
        var facilityType = await _unitOfWork.FacilityTypes.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(FacilityType), request.Id);

        if (facilityType.IsDeleted)
        {
            throw new NotFoundException(nameof(FacilityType), request.Id);
        }

        facilityType.TypeName = request.TypeName;
        facilityType.Description = request.Description;
        facilityType.IsActive = request.IsActive;
        facilityType.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FacilityTypeDto(
            facilityType.Id,
            facilityType.TypeCode,
            facilityType.TypeName,
            facilityType.Description,
            facilityType.IsActive
        );
    }
}
