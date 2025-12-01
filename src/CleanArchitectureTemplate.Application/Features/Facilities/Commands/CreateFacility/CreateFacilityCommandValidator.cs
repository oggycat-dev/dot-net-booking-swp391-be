using FluentValidation;
using CleanArchitectureTemplate.Application.Common.Interfaces;

namespace CleanArchitectureTemplate.Application.Features.Facilities.Commands.CreateFacility;

public class CreateFacilityCommandValidator : AbstractValidator<CreateFacilityCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFacilityCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.FacilityCode)
            .NotEmpty().WithMessage("Facility code is required")
            .MaximumLength(20).WithMessage("Facility code cannot exceed 20 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Facility code must contain only uppercase letters, numbers, and hyphens")
            .MustAsync(BeUniqueCode).WithMessage("Facility code already exists");

        RuleFor(x => x.FacilityName)
            .NotEmpty().WithMessage("Facility name is required")
            .MaximumLength(200).WithMessage("Facility name cannot exceed 200 characters");

        RuleFor(x => x.TypeId)
            .NotEmpty().WithMessage("Facility type is required")
            .MustAsync(FacilityTypeExists).WithMessage("Facility type does not exist");

        RuleFor(x => x.CampusId)
            .NotEmpty().WithMessage("Campus is required")
            .MustAsync(CampusExists).WithMessage("Campus does not exist");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Capacity cannot exceed 1000");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Equipment)
            .MaximumLength(500).WithMessage("Equipment cannot exceed 500 characters");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters");
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Facilities.IsCodeUniqueAsync(code);
    }

    private async Task<bool> FacilityTypeExists(Guid facilityTypeId, CancellationToken cancellationToken)
    {
        var facilityType = await _unitOfWork.FacilityTypes.GetByIdAsync(facilityTypeId);
        return facilityType != null && !facilityType.IsDeleted && facilityType.IsActive;
    }

    private async Task<bool> CampusExists(Guid campusId, CancellationToken cancellationToken)
    {
        var campus = await _unitOfWork.Campuses.GetByIdAsync(campusId);
        return campus != null && !campus.IsDeleted && campus.IsActive;
    }
}
