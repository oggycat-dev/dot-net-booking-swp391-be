using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class FacilityRepository : Repository<Facility>, IFacilityRepository
{
    public FacilityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Facility>> GetByCampusIdAsync(Guid campusId)
    {
        return await _context.Facilities
            .Include(f => f.Type)
            .Include(f => f.Campus)
            .Where(f => !f.IsDeleted && f.CampusId == campusId)
            .OrderBy(f => f.FacilityCode)
            .ToListAsync();
    }

    public async Task<List<Facility>> GetByFacilityTypeIdAsync(Guid facilityTypeId)
    {
        return await _context.Facilities
            .Include(f => f.Type)
            .Include(f => f.Campus)
            .Where(f => !f.IsDeleted && f.TypeId == facilityTypeId)
            .OrderBy(f => f.FacilityCode)
            .ToListAsync();
    }

    public async Task<List<Facility>> GetAvailableFacilitiesAsync(Guid? campusId = null, Guid? facilityTypeId = null)
    {
        var query = _context.Facilities
            .Include(f => f.Type)
            .Include(f => f.Campus)
            .Where(f => !f.IsDeleted && f.IsActive && f.Status == Domain.Enums.FacilityStatus.Available);

        if (campusId.HasValue)
        {
            query = query.Where(f => f.CampusId == campusId.Value);
        }

        if (facilityTypeId.HasValue)
        {
            query = query.Where(f => f.TypeId == facilityTypeId.Value);
        }

        return await query
            .OrderBy(f => f.Campus.CampusName)
            .ThenBy(f => f.Type.TypeName)
            .ThenBy(f => f.FacilityCode)
            .ToListAsync();
    }

    public async Task<Facility?> GetByCodeAsync(string facilityCode)
    {
        return await _context.Facilities
            .Include(f => f.Type)
            .Include(f => f.Campus)
            .FirstOrDefaultAsync(f => !f.IsDeleted && f.FacilityCode == facilityCode);
    }

    public async Task<bool> IsCodeUniqueAsync(string facilityCode, Guid? excludeId = null)
    {
        var query = _context.Facilities.Where(f => !f.IsDeleted && f.FacilityCode == facilityCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }
}
