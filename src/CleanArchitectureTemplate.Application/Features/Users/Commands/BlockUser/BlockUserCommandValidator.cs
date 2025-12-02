using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.BlockUser;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Block reason is required")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.BlockedUntil)
            .GreaterThan(DateTime.UtcNow).WithMessage("Block until date must be in the future")
            .When(x => x.BlockedUntil.HasValue);
    }
}
