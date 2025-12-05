using CleanArchitectureTemplate.Application.Common.DTOs.Facility;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using System.Text.Json;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.UpdateFacility;

public class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, FacilityDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public UpdateFacilityCommandHandler(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<FacilityDto> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
    {
        var facility = await _unitOfWork.Facilities.GetByIdAsync(request.Id)
            ?? throw new NotFoundException(nameof(Facility), request.Id);

        if (facility.IsDeleted)
        {
            throw new NotFoundException(nameof(Facility), request.Id);
        }

        // Upload new images to Cloudinary if provided
        if (request.Images != null && request.Images.Any())
        {
            var imageUrls = new List<string>();
            
            // If there are existing images, parse them and add to the list
            if (!string.IsNullOrEmpty(facility.ImageUrl))
            {
                try
                {
                    var existingUrls = JsonSerializer.Deserialize<List<string>>(facility.ImageUrl);
                    if (existingUrls != null)
                    {
                        imageUrls.AddRange(existingUrls);
                    }
                }
                catch
                {
                    // If parsing fails, treat as single URL
                    imageUrls.Add(facility.ImageUrl);
                }
            }
            
            // Upload new images
            foreach (var image in request.Images)
            {
                using var stream = image.OpenReadStream();
                var url = await _cloudinaryService.UploadImageAsync(stream, image.FileName);
                imageUrls.Add(url);
            }
            
            facility.ImageUrl = JsonSerializer.Serialize(imageUrls);
        }
        else if (request.ImageUrl != null)
        {
            // If ImageUrl is explicitly provided (not from file upload), use it
            facility.ImageUrl = request.ImageUrl;
        }

        facility.FacilityName = request.FacilityName;
        facility.TypeId = request.TypeId;
        facility.Building = request.Building;
        facility.Floor = request.Floor;
        facility.RoomNumber = request.RoomNumber;
        facility.Capacity = request.Capacity;
        facility.Description = request.Description;
        facility.Equipment = request.Equipment;
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
