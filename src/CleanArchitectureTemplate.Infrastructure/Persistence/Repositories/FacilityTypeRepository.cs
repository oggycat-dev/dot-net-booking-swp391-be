using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class FacilityTypeRepository : Repository<FacilityType>, IFacilityTypeRepository
{
    public FacilityTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<FacilityType?> GetByCodeAsync(string typeCode)
    {
        return await _context.FacilityTypes
            .FirstOrDefaultAsync(ft => !ft.IsDeleted && ft.TypeCode == typeCode);
    }

    public async Task<bool> IsCodeUniqueAsync(string typeCode, Guid? excludeId = null)
    {
        var query = _context.FacilityTypes.Where(ft => !ft.IsDeleted && ft.TypeCode == typeCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(ft => ft.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<List<FacilityType>> GetActiveTypesAsync()
    {
        return await _context.FacilityTypes
            .Where(ft => !ft.IsDeleted && ft.IsActive)
            .OrderBy(ft => ft.TypeName)
            .ToListAsync();
    }
}
