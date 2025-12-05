using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

namespace CleanArchitectureTemplate.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IUserRepository? _userRepository;
    private ICampusRepository? _campusRepository;
    private IFacilityRepository? _facilityRepository;
    private IFacilityTypeRepository? _facilityTypeRepository;
    private ICampusChangeRequestRepository? _campusChangeRequestRepository;
    private IBookingRepository? _bookingRepository;
    private IHolidayRepository? _holidayRepository;
    private IFacilityIssueReportRepository? _facilityIssueReportRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public ICampusRepository Campuses => _campusRepository ??= new CampusRepository(_context);
    public IFacilityRepository Facilities => _facilityRepository ??= new FacilityRepository(_context);
    public IFacilityTypeRepository FacilityTypes => _facilityTypeRepository ??= new FacilityTypeRepository(_context);
    public ICampusChangeRequestRepository CampusChangeRequests => _campusChangeRequestRepository ??= new CampusChangeRequestRepository(_context);
    public IBookingRepository Bookings => _bookingRepository ??= new BookingRepository(_context);
    public IHolidayRepository Holidays => _holidayRepository ??= new HolidayRepository(_context);
    public IFacilityIssueReportRepository FacilityIssueReports => _facilityIssueReportRepository ??= new FacilityIssueReportRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}