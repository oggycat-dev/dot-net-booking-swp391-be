using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPasswordWithCode;

public record ResetPasswordWithCodeCommand(
    string Email,
    string VerificationCode,
    string NewPassword
) : IRequest<Unit>;
