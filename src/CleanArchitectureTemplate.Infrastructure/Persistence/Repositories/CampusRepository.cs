using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class CampusRepository : Repository<Campus>, ICampusRepository
{
    public CampusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Campus?> GetByCodeAsync(string code)
    {
        return await _context.Set<Campus>()
            .FirstOrDefaultAsync(c => c.CampusCode == code);
    }

    public async Task<bool> IsCodeExistsAsync(string code)
    {
        return await _context.Set<Campus>()
            .AnyAsync(c => c.CampusCode == code);
    }

    public async Task<List<Campus>> GetActiveCampusesAsync()
    {
        return await _context.Set<Campus>()
            .Where(c => c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.CampusName)
            .ToListAsync();
    }
}
