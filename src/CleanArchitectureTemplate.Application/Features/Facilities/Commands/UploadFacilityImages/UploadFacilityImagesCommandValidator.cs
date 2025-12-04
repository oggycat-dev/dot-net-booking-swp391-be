using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.UploadFacilityImages;

public class UploadFacilityImagesCommandValidator : AbstractValidator<UploadFacilityImagesCommand>
{
    public UploadFacilityImagesCommandValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty()
            .WithMessage("Facility ID is required");

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one image is required")
            .Must(images => images.Count <= 10)
            .WithMessage("Maximum 10 images can be uploaded at once");
    }
}
