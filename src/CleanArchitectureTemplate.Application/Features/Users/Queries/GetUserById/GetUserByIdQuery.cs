using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;