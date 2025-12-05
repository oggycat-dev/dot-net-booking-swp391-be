using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class FacilityIssueReportRepository : Repository<FacilityIssueReport>, IFacilityIssueReportRepository
{
    public FacilityIssueReportRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<FacilityIssueReport>> GetPendingReportsAsync()
    {
        return await _context.Set<FacilityIssueReport>()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.ReportedByUser)
            .Where(r => !r.IsDeleted && (r.Status == "Pending" || r.Status == "UnderReview"))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<FacilityIssueReport>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Set<FacilityIssueReport>()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.ReportedByUser)
            .Include(r => r.NewFacility)
            .Where(r => !r.IsDeleted && r.ReportedBy == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<FacilityIssueReport>> GetByBookingIdAsync(Guid bookingId)
    {
        return await _context.Set<FacilityIssueReport>()
            .Include(r => r.Booking)
                .ThenInclude(b => b.Facility)
            .Include(r => r.ReportedByUser)
            .Include(r => r.NewFacility)
            .Where(r => !r.IsDeleted && r.BookingId == bookingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
