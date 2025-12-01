using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.UpdateFacility;

public class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, FacilityDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFacilityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FacilityDto> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = await _unitOfWork.Facilities.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Facility), request.Id);

        if (facility.IsDeleted)
        {
            throw new NotFoundException(nameof(Facility), request.Id);
        }

        facility.FacilityName = request.FacilityName;
        facility.TypeId = request.TypeId;
        facility.Building = request.Building;
        facility.Floor = request.Floor;
        facility.RoomNumber = request.RoomNumber;
        facility.Capacity = request.Capacity;
        facility.Description = request.Description;
        facility.Equipment = request.Equipment;
        facility.ImageUrl = request.ImageUrl;
        facility.Status = Enum.Parse<FacilityStatus>(request.Status);
        facility.IsActive = request.IsActive;
        facility.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with related data
        var updatedFacility = await _unitOfWork.Facilities.GetByIdAsync(facility.Id);

        return new FacilityDto(
            updatedFacility!.Id,
            updatedFacility.FacilityCode,
            updatedFacility.FacilityName,
            updatedFacility.TypeId,
            updatedFacility.Type?.TypeName ?? string.Empty,
            updatedFacility.CampusId,
            updatedFacility.Campus?.CampusName ?? string.Empty,
            updatedFacility.Building,
            updatedFacility.Floor,
            updatedFacility.RoomNumber,
            updatedFacility.Capacity,
            updatedFacility.Description,
            updatedFacility.Equipment,
            updatedFacility.ImageUrl,
            updatedFacility.Status.ToString(),
            updatedFacility.IsActive
        );
    }
}
