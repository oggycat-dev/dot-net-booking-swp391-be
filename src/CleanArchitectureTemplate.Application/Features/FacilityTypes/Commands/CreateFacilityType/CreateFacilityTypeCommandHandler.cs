using CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.CreateFacilityType;

public class CreateFacilityTypeCommandHandler : IRequestHandler<CreateFacilityTypeCommand, FacilityTypeDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFacilityTypeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FacilityTypeDto> Handle(CreateFacilityTypeCommand request, CancellationToken cancellationToken)
    {
        var facilityType = new FacilityType
        {
            Id = Guid.NewGuid(),
            TypeCode = request.TypeCode,
            TypeName = request.TypeName,
            Description = request.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FacilityTypes.AddAsync(facilityType);
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
