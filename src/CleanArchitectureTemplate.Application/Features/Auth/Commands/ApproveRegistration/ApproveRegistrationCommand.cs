using CleanArchitectureTemplate.Application.Common.DTOs;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Commands.ApproveRegistration;

public record ApproveRegistrationCommand(
    Guid UserId,
    bool IsApproved,
    string? RejectionReason
) : IRequest<ApiResponse<bool>>;
