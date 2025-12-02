using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.ResetPassword;

public record ResetPasswordCommand(
    Guid UserId,
    string NewPassword
) : IRequest<Unit>;
