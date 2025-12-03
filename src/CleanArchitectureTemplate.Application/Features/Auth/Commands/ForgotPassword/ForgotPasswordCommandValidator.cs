using FluentValidation;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .Must(email => email.EndsWith("@fpt.edu.vn")).WithMessage("Email must be @fpt.edu.vn domain");
    }
}
