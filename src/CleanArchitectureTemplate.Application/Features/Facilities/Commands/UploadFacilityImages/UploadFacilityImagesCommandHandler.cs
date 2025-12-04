using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.UploadFacilityImages;

public class UploadFacilityImagesCommandHandler : IRequestHandler<UploadFacilityImagesCommand, List<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public UploadFacilityImagesCommandHandler(
        IUnitOfWork unitOfWork,
        ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<List<string>> Handle(UploadFacilityImagesCommand request, CancellationToken cancellationToken)
    {
        // Validate facility exists
        var facility = await _unitOfWork.Facilities.GetByIdAsync(request.FacilityId)
            ?? throw new NotFoundException(nameof(Facility), request.FacilityId);

        // Validate images
        if (request.Images == null || request.Images.Count == 0)
        {
            throw new ValidationException("At least one image is required");
        }

        // Validate file types and sizes
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var maxFileSize = 5 * 1024 * 1024; // 5MB

        foreach (var image in request.Images)
        {
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                throw new ValidationException($"Invalid file type: {extension}. Allowed types: {string.Join(", ", allowedExtensions)}");
            }

            if (image.Length > maxFileSize)
            {
                throw new ValidationException($"File {image.FileName} exceeds maximum size of 5MB");
            }
        }

        // Upload images to Cloudinary
        var uploadTasks = new List<(Stream, string)>();
        
        foreach (var image in request.Images)
        {
            var stream = image.OpenReadStream();
            var fileName = $"{facility.FacilityCode}_{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            uploadTasks.Add((stream, fileName));
        }

        var imageUrls = await _cloudinaryService.UploadMultipleImagesAsync(uploadTasks);

        // Update facility with new image URLs
        var currentImages = string.IsNullOrEmpty(facility.ImageUrl) 
            ? new List<string>() 
            : facility.ImageUrl.Split(',').ToList();
        
        currentImages.AddRange(imageUrls);
        facility.ImageUrl = string.Join(",", currentImages);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return imageUrls;
    }
}
