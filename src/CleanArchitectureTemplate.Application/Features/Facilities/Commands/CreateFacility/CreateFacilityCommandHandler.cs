using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.CreateFacility;

public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, FacilityDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFacilityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FacilityDto> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = new Facility
        {
            Id = Guid.NewGuid(),
            FacilityCode = request.FacilityCode,
            FacilityName = request.FacilityName,
            TypeId = request.TypeId,
            CampusId = request.CampusId,
            Building = request.Building,
            Floor = request.Floor,
            RoomNumber = request.RoomNumber,
            Capacity = request.Capacity,
            Description = request.Description,
            Equipment = request.Equipment,
            ImageUrl = request.ImageUrl,
            Status = FacilityStatus.Available,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Facilities.AddAsync(facility);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with related data
        var createdFacility = await _unitOfWork.Facilities.GetByIdAsync(facility.Id);
        
        return new FacilityDto(
            createdFacility!.Id,
            createdFacility.FacilityCode,
            createdFacility.FacilityName,
            createdFacility.TypeId,
            createdFacility.Type?.TypeName ?? string.Empty,
            createdFacility.CampusId,
            createdFacility.Campus?.CampusName ?? string.Empty,
            createdFacility.Building,
            createdFacility.Floor,
            createdFacility.RoomNumber,
            createdFacility.Capacity,
            createdFacility.Description,
            createdFacility.Equipment,
            createdFacility.ImageUrl,
            createdFacility.Status.ToString(),
            createdFacility.IsActive
        );
    }
}
