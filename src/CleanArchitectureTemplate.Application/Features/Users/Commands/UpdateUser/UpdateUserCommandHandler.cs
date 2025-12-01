using AutoMapper;
using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.ValueObjects;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"User with ID {request.Id} not found");

        // Validate unique email if changed
        if (user.Email != request.Email)
        {
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => 
                u.Email == request.Email && u.Id != request.Id);

            if (existingUser != null)
            {
                throw new ValidationException("Email already exists");
            }
        }

        var email = new Email(request.Email);
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = email;
        user.Role = request.Role;
        user.IsActive = request.IsActive;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}