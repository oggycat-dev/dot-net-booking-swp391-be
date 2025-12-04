using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Persistence.Repositories;

public class HolidayRepository : Repository<Holiday>, IHolidayRepository
{
    public HolidayRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> IsHolidayAsync(DateTime date)
    {
        var dateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        
        // Check for exact date match (non-recurring holidays)
        var exactMatch = await _context.Holidays
            .Where(h => !h.IsDeleted && h.HolidayDate == dateOnly)
            .AnyAsync();

        if (exactMatch) return true;

        // Check for recurring holidays (same month and day)
        var recurringMatch = await _context.Holidays
            .Where(h => !h.IsDeleted && 
                       h.IsRecurring && 
                       h.HolidayDate.Month == date.Month && 
                       h.HolidayDate.Day == date.Day)
            .AnyAsync();

        return recurringMatch;
    }

    public async Task<List<Holiday>> GetUpcomingHolidaysAsync(int days = 30)
    {
        var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var futureDate = today.AddDays(days);

        return await _context.Holidays
            .Where(h => !h.IsDeleted && h.HolidayDate >= today && h.HolidayDate <= futureDate)
            .OrderBy(h => h.HolidayDate)
            .ToListAsync();
    }
}
