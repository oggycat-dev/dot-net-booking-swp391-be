using CleanArchitectureTemplate.Domain.Entities;

namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserCodeAsync(string userCode);
    Task<bool> IsEmailExistsAsync(string email);
    Task<bool> IsUserCodeExistsAsync(string userCode);
}

public interface ICampusRepository : IRepository<Campus>
{
    Task<Campus?> GetByCodeAsync(string code);
    Task<bool> IsCodeExistsAsync(string code);
    Task<List<Campus>> GetActiveCampusesAsync();
}

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICampusRepository Campuses { get; }
    IFacilityRepository Facilities { get; }
    IFacilityTypeRepository FacilityTypes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}