using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateMyProfileCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        // Get current user
        var userId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        // Update profile information
        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.Department = request.Department;
        user.Major = request.Major;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
