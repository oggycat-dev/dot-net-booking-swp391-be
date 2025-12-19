using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Booking>> GetWaitingLecturerApprovalByEmailAsync(string lecturerEmail)
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Facility)
                .ThenInclude(f => f.Campus)
            .Where(b => b.LecturerEmail == lecturerEmail && 
                       b.Status == BookingStatus.WaitingLecturerApproval && 
                       !b.IsDeleted)
            .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetPendingAdminApprovalsAsync()
    {
        return await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Facility)
                .ThenInclude(f => f.Campus)
            .Include(b => b.LecturerApprover)
            .Where(b => b.Status == BookingStatus.Pending && !b.IsDeleted)
            .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetApprovedBookingsAsync(Guid? facilityId = null, Guid? campusId = null, DateTime? fromDate = null, DateTime? toDate = null, string? searchTerm = null)
    {
        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Facility)
                .ThenInclude(f => f.Campus)
            .Include(b => b.Facility)
                .ThenInclude(f => f.Type)
            .Include(b => b.Approver)
            .Where(b => (b.Status == BookingStatus.Approved || b.Status == BookingStatus.InUse) && !b.IsDeleted)
            .AsQueryable();

        if (facilityId.HasValue)
        {
            query = query.Where(b => b.FacilityId == facilityId.Value);
        }

        if (campusId.HasValue)
        {
            query = query.Where(b => b.Facility.CampusId == campusId.Value);
        }

        if (fromDate.HasValue)
        {
            var fromDateUtc = DateTime.SpecifyKind(fromDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(b => b.BookingDate >= fromDateUtc);
        }

        if (toDate.HasValue)
        {
            var toDateUtc = DateTime.SpecifyKind(toDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(b => b.BookingDate <= toDateUtc);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            query = query.Where(b =>
                b.BookingCode.ToLower().Contains(searchTermLower) ||
                b.User.FullName.ToLower().Contains(searchTermLower) ||
                b.Facility.FacilityName.ToLower().Contains(searchTermLower) ||
                b.Purpose.ToLower().Contains(searchTermLower)
            );
        }

        return await query
            .OrderByDescending(b => b.BookingDate)
                .ThenByDescending(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetAllBookingsForAdminAsync(Guid? facilityId = null, Guid? campusId = null, DateTime? fromDate = null, DateTime? toDate = null, string? status = null, string? searchTerm = null)
    {
        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Facility)
                .ThenInclude(f => f.Campus)
            .Include(b => b.Facility)
                .ThenInclude(f => f.Type)
            .Include(b => b.LecturerApprover)
            .Include(b => b.Approver)
            .Where(b => !b.IsDeleted)
            .AsQueryable();

        if (facilityId.HasValue)
        {
            query = query.Where(b => b.FacilityId == facilityId.Value);
        }

        if (campusId.HasValue)
        {
            query = query.Where(b => b.Facility.CampusId == campusId.Value);
        }

        if (fromDate.HasValue)
        {
            var fromDateUtc = DateTime.SpecifyKind(fromDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(b => b.BookingDate >= fromDateUtc);
        }

        if (toDate.HasValue)
        {
            var toDateUtc = DateTime.SpecifyKind(toDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(b => b.BookingDate <= toDateUtc);
        }

        // Filter by status if provided
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<BookingStatus>(status, true, out var bookingStatus))
            {
                query = query.Where(b => b.Status == bookingStatus);
            }
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            query = query.Where(b =>
                b.BookingCode.ToLower().Contains(searchTermLower) ||
                b.User.FullName.ToLower().Contains(searchTermLower) ||
                b.User.Email.ToLower().Contains(searchTermLower) ||
                b.Facility.FacilityName.ToLower().Contains(searchTermLower) ||
                b.Purpose.ToLower().Contains(searchTermLower)
            );
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.Facility)
                .ThenInclude(f => f.Campus)
            .Include(b => b.LecturerApprover)
            .Include(b => b.Approver)
            .Where(b => b.UserId == userId && !b.IsDeleted)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetMyPendingBookingsAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.Facility)
                .ThenInclude(f => f.Campus)
            .Include(b => b.LecturerApprover)
            .Include(b => b.Approver)
            .Where(b => b.UserId == userId && 
                       (b.Status == BookingStatus.WaitingLecturerApproval || 
                        b.Status == BookingStatus.Pending) && 
                       !b.IsDeleted)
            .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(Guid facilityId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime, Guid? excludeBookingId = null)
    {
        // Ensure bookingDate is UTC for PostgreSQL comparison
        var dateOnly = DateTime.SpecifyKind(bookingDate.Date, DateTimeKind.Utc);
        
        var query = _context.Bookings
            .Where(b => b.FacilityId == facilityId &&
                       b.BookingDate == dateOnly &&
                       b.Status != BookingStatus.Rejected &&
                       b.Status != BookingStatus.Cancelled &&
                       !b.IsDeleted);

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        var existingBookings = await query.ToListAsync();

        return existingBookings.Any(b => 
            b.StartTime < endTime && b.EndTime > startTime
        );
    }
}
