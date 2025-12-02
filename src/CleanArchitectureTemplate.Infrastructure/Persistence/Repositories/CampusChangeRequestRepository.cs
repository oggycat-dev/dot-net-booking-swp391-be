using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class CampusChangeRequestRepository : Repository<CampusChangeRequest>, ICampusChangeRequestRepository
{
    public CampusChangeRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<CampusChangeRequest>> GetPendingRequestsAsync()
    {
        return await _context.CampusChangeRequests
            .Include(r => r.User)
            .Include(r => r.CurrentCampus)
            .Include(r => r.RequestedCampus)
            .Where(r => r.Status == CampusChangeRequestStatus.Pending && !r.IsDeleted)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<CampusChangeRequest>> GetByUserIdAsync(Guid userId)
    {
        return await _context.CampusChangeRequests
            .Include(r => r.CurrentCampus)
            .Include(r => r.RequestedCampus)
            .Include(r => r.ReviewedByAdmin)
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasPendingRequestAsync(Guid userId)
    {
        return await _context.CampusChangeRequests
            .AnyAsync(r => r.UserId == userId && 
                          r.Status == CampusChangeRequestStatus.Pending && 
                          !r.IsDeleted);
    }
}
