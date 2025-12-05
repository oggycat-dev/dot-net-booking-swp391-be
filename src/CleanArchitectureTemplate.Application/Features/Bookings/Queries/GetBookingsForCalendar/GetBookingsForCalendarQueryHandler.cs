using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetBookingsForCalendar;

public class GetBookingsForCalendarQueryHandler : IRequestHandler<GetBookingsForCalendarQuery, List<BookingCalendarDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBookingsForCalendarQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BookingCalendarDto>> Handle(GetBookingsForCalendarQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Bookings.GetQueryable()
            .Include(b => b.Facility)
                .ThenInclude(f => f!.Campus)
            .Include(b => b.User)
            .Where(b => !b.IsDeleted &&
                       b.BookingDate >= request.StartDate.Date &&
                       b.BookingDate <= request.EndDate.Date &&
                       // Include only bookings that occupy time slots
                       (b.Status == BookingStatus.WaitingLecturerApproval ||
                        b.Status == BookingStatus.Pending ||
                        b.Status == BookingStatus.Approved ||
                        b.Status == BookingStatus.InUse));

        // Filter by facility if provided
        if (request.FacilityId.HasValue)
        {
            query = query.Where(b => b.FacilityId == request.FacilityId.Value);
        }

        // Filter by campus if provided
        if (request.CampusId.HasValue)
        {
            query = query.Where(b => b.Facility!.CampusId == request.CampusId.Value);
        }

        var bookings = await query
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToListAsync(cancellationToken);

        return bookings.Select(b => new BookingCalendarDto(
            b.Id,
            b.BookingCode,
            b.FacilityId,
            b.Facility?.FacilityName ?? string.Empty,
            b.Facility?.FacilityCode ?? string.Empty,
            b.Facility?.Campus?.CampusName ?? string.Empty,
            b.User?.FullName ?? string.Empty,
            b.User?.Role.ToString() ?? string.Empty,
            b.BookingDate,
            b.StartTime,
            b.EndTime,
            b.Status.ToString(),
            b.Purpose,
            b.NumParticipants
        )).ToList();
    }
}
