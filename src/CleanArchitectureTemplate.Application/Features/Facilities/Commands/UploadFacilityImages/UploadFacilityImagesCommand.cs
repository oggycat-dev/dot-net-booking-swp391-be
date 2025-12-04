using MediatR;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.UploadFacilityImages;

public class UploadFacilityImagesCommand : IRequest<List<string>>
{
    public Guid FacilityId { get; set; }
    public List<IFormFile> Images { get; set; } = new();
}
