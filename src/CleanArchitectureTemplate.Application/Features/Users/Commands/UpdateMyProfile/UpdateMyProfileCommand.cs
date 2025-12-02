using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Users.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(
    string FullName,
    string? PhoneNumber,
    string? Department,
    string? Major
) : IRequest<Unit>;
