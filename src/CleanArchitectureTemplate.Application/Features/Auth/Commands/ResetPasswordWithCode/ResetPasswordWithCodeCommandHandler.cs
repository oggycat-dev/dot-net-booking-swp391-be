using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ResetPasswordWithCode;

public class ResetPasswordWithCodeCommandHandler : IRequestHandler<ResetPasswordWithCodeCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordWithCodeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ResetPasswordWithCodeCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email)
            ?? throw new ValidationException("Invalid email or verification code");

        // Check if verification code exists
        if (string.IsNullOrEmpty(user.PasswordResetCode))
        {
            throw new ValidationException("No password reset request found. Please request a new code.");
        }

        // Check if verification code matches
        if (user.PasswordResetCode != request.VerificationCode)
        {
            throw new ValidationException("Invalid verification code");
        }

        // Check if code has expired
        if (user.PasswordResetCodeExpiry == null || user.PasswordResetCodeExpiry < DateTime.UtcNow)
        {
            throw new ValidationException("Verification code has expired. Please request a new code.");
        }

        // Reset password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        
        // Clear verification code
        user.PasswordResetCode = null;
        user.PasswordResetCodeExpiry = null;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
