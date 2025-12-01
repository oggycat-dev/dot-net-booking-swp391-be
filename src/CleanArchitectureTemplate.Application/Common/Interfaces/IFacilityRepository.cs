using CleanArchitectureTemplate.Domain.Entities;

namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface IFacilityRepository : IRepository<Facility>
{
    Task<List<Facility>> GetByCampusIdAsync(Guid campusId);
    Task<List<Facility>> GetByFacilityTypeIdAsync(Guid facilityTypeId);
    Task<List<Facility>> GetAvailableFacilitiesAsync(Guid? campusId = null, Guid? facilityTypeId = null);
    Task<Facility?> GetByCodeAsync(string facilityCode);
    Task<bool> IsCodeUniqueAsync(string facilityCode, Guid? excludeId = null);
}
