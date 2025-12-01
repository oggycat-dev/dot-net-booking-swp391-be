using FluentValidation;
using CleanArchitectureTemplate.Application.Common.Interfaces;

namespace CleanArchitectureTemplate.Application.Features.FacilityTypes.Commands.CreateFacilityType;

public class CreateFacilityTypeCommandValidator : AbstractValidator<CreateFacilityTypeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFacilityTypeCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.TypeCode)
            .NotEmpty().WithMessage("Type code is required")
            .MaximumLength(20).WithMessage("Type code cannot exceed 20 characters")
            .Matches("^[A-Z0-9-]+$").WithMessage("Type code must contain only uppercase letters, numbers, and hyphens")
            .MustAsync(BeUniqueCode).WithMessage("Type code already exists");

        RuleFor(x => x.TypeName)
            .NotEmpty().WithMessage("Type name is required")
            .MaximumLength(100).WithMessage("Type name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return await _unitOfWork.FacilityTypes.IsCodeUniqueAsync(code);
    }
}
