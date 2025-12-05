using CleanArchitectureTemplate.Application.Common.DTOs.Dashboard;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Dashboard.Queries.GetAdminDashboard;

/// <summary>
/// Query to get admin dashboard statistics
/// </summary>
public record GetAdminDashboardQuery : IRequest<AdminDashboardDto>
{
}
