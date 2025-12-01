using CleanArchitectureTemplate.Application.Common.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword,
    string PhoneNumber,
    Guid CampusId,
    string Role,
    string? Department,
    string? Major
) : IRequest<ApiResponse<Guid>>;
