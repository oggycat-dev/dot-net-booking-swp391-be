using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using System.Text.Json;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.CreateFacility;

public class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, FacilityDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public CreateFacilityCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<FacilityDto> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        // Upload images to Cloudinary if provided
        string? imageUrlJson = null;
        if (request.Images != null && request.Images.Any())
        {
            var imageUrls = new List<string>();
            foreach (var image in request.Images)
            {
                using var stream = image.OpenReadStream();
                var url = await _cloudinaryService.UploadImageAsync(stream, image.FileName);
                imageUrls.Add(url);
            }
            // Store as JSON array
            imageUrlJson = JsonSerializer.Serialize(imageUrls);
        }

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
            ImageUrl = imageUrlJson,
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
