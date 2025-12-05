using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.UpdateFacilityType;

public class UpdateFacilityTypeCommandValidator : AbstractValidator<UpdateFacilityTypeCommand>
{
    public UpdateFacilityTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.TypeName)
            .NotEmpty().WithMessage("TypeName is required")
            .MaximumLength(100).WithMessage("TypeName must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}
