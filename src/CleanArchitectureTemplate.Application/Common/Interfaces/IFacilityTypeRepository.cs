using CleanArchitectureTemplate.Domain.Entities;

namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface IFacilityTypeRepository : IRepository<FacilityType>
{
    Task<FacilityType?> GetByCodeAsync(string typeCode);
    Task<bool> IsCodeUniqueAsync(string typeCode, Guid? excludeId = null);
    Task<List<FacilityType>> GetActiveTypesAsync();
}
