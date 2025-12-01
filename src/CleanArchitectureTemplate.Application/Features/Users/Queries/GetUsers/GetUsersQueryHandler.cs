using MediatR;
using AutoMapper;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.Models;
using CleanArchitectureTemplate.Domain.Enums;
using UserDto = CleanArchitectureTemplate.Application.Common.DTOs.Users.UserDto;

namespace CleanArchitectureTemplate.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Get users query handler
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // Build filter expression
        var users = await _unitOfWork.Users.GetAllAsync();
        
        var query = users.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u => 
                u.FirstName.Contains(request.SearchTerm) ||
                u.LastName.Contains(request.SearchTerm) ||
                u.Email.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, out var role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        // Get total count
        var totalCount = query.Count();

        // Apply pagination
        var paginatedUsers = query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Map to DTOs
        var userDtos = _mapper.Map<List<UserDto>>(paginatedUsers);

        return new PaginatedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
