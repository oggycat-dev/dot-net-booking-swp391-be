using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UnblockUser;

public class UnblockUserCommandValidator : AbstractValidator<UnblockUserCommand>
{
    public UnblockUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
