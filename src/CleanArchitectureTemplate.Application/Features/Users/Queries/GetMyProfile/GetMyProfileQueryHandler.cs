using AutoMapper;
using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMyProfileQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        // Get current user
        var userId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        return _mapper.Map<UserDto>(user);
    }
}
