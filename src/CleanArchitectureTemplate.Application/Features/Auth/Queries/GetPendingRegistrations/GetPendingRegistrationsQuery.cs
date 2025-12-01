using CleanArchitectureTemplate.Application.Common.DTOs.Auth;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Auth.Queries.GetPendingRegistrations;

public record GetPendingRegistrationsQuery : IRequest<List<PendingRegistrationDto>>;
