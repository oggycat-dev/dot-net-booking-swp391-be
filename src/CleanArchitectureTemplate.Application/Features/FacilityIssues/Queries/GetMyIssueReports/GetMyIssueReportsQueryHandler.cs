using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Queries.GetMyIssueReports;

public class GetMyIssueReportsQueryHandler : IRequestHandler<GetMyIssueReportsQuery, List<FacilityIssueReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyIssueReportsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<List<FacilityIssueReportDto>> Handle(GetMyIssueReportsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var reports = await _unitOfWork.FacilityIssueReports.GetQueryable()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.ReportedByUser)
            .Include(r => r.NewFacility)
            .Where(r => !r.IsDeleted && r.ReportedBy == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return reports.Select(r => {
            List<string>? imageUrls = null;
            if (!string.IsNullOrEmpty(r.ImageUrls))
            {
                try
                {
                    imageUrls = JsonSerializer.Deserialize<List<string>>(r.ImageUrls);
                }
                catch
                {
                    imageUrls = new List<string> { r.ImageUrls };
                }
            }
            
            return new FacilityIssueReportDto(
                r.Id,
                r.ReportCode,
                r.BookingId,
                r.Booking.BookingCode,
                r.Booking.FacilityId,
                r.Booking.Facility.FacilityName,
                r.ReportedByUser.FullName,
                r.ReportedByUser.Email,
                r.IssueTitle,
                r.IssueDescription,
                r.Severity,
                r.Category,
                imageUrls,
                r.Status,
                r.NewFacilityId,
                r.NewFacility?.FacilityName,
                r.AdminResponse,
                r.CreatedAt,
                r.HandledAt,
                r.ResolvedAt
            );
        }).ToList();
    }
}
