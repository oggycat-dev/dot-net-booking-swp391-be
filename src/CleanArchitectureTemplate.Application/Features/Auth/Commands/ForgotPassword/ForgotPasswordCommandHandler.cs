using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        
        // Don't reveal if user exists or not for security reasons
        if (user == null)
        {
            return Unit.Value;
        }

        // Check if user is active and approved
        if (!user.IsActive || !user.IsApproved)
        {
            return Unit.Value;
        }

        // Generate 6-digit verification code
        var random = new Random();
        var verificationCode = random.Next(100000, 999999).ToString();

        // Store verification code with expiry (15 minutes)
        user.PasswordResetCode = verificationCode;
        user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send email with verification code
        try
        {
            await _emailService.SendPasswordResetCodeEmailAsync(user.Email, user.FullName, verificationCode);
        }
        catch
        {
            // Log error but don't fail the operation
            // User should still be able to use the code if email fails
        }

        return Unit.Value;
    }
}
