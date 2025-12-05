using CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CleanArchitectureTemplate.Application.Features.FacilityIssues.Queries.GetPendingIssueReports;

public class GetPendingIssueReportsQueryHandler : IRequestHandler<GetPendingIssueReportsQuery, List<FacilityIssueReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingIssueReportsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FacilityIssueReportDto>> Handle(GetPendingIssueReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _unitOfWork.FacilityIssueReports.GetQueryable()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.ReportedByUser)
            .Include(r => r.NewFacility)
            .Where(r => !r.IsDeleted && (r.Status == "Pending" || r.Status == "UnderReview"))
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
