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

public interface ICampusChangeRequestRepository : IRepository<CampusChangeRequest>
{
    Task<List<CampusChangeRequest>> GetPendingRequestsAsync();
    Task<List<CampusChangeRequest>> GetByUserIdAsync(Guid userId);
    Task<bool> HasPendingRequestAsync(Guid userId);
}

public interface IBookingRepository : IRepository<Booking>
{
    Task<List<Booking>> GetWaitingLecturerApprovalByEmailAsync(string lecturerEmail);
    Task<List<Booking>> GetPendingAdminApprovalsAsync();
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<List<Booking>> GetMyPendingBookingsAsync(Guid userId);
    Task<bool> HasConflictAsync(Guid facilityId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime, Guid? excludeBookingId = null);
}

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICampusRepository Campuses { get; }
    IFacilityRepository Facilities { get; }
    IFacilityTypeRepository FacilityTypes { get; }
    ICampusChangeRequestRepository CampusChangeRequests { get; }
    IBookingRepository Bookings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}