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
