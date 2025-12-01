using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Application.Common.Interfaces;

/// <summary>
/// Application database context interface
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Users DbSet
    /// </summary>
    DbSet<Domain.Entities.User> Users { get; }
    
    /// <summary>
    /// Save changes asynchronously
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected records</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
