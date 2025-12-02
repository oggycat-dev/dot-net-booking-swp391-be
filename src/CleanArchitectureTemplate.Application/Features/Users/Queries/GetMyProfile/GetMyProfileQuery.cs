using CleanArchitectureTemplate.Application.Common.DTOs.Users;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<UserDto>;
