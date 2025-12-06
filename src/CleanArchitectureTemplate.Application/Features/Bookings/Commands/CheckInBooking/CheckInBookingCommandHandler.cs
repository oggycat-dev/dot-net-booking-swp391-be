using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Helpers;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CheckInBooking;

public class CheckInBookingCommandHandler : IRequestHandler<CheckInBookingCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CheckInBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CheckInBookingCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new ValidationException("User not authenticated");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // Only booking owner can check-in
        if (booking.UserId != userId)
        {
            throw new ValidationException("You can only check-in your own bookings");
        }

        // Check if booking is approved
        if (booking.Status != BookingStatus.Approved)
        {
            throw new ValidationException($"Cannot check-in. Booking status is {booking.Status}");
        }

        // Check if already checked in
        if (booking.CheckedInAt.HasValue)
        {
            throw new ValidationException("This booking has already been checked in");
        }

        // Get current Vietnam time (GMT+7) for comparison
        var nowVietnam = TimeZoneHelper.GetVietnamNow();
        
        // Booking date and time are stored as local time (treat as Vietnam time)
        // Simply combine date + time for comparison in Vietnam timezone
        var bookingDateTime = booking.BookingDate.Date.Add(booking.StartTime);
        
        // Check-in window: from start time to 15 minutes after start time (in Vietnam time)
        var checkInWindowStart = bookingDateTime;
        var checkInWindowEnd = bookingDateTime.AddMinutes(15);

        if (nowVietnam < checkInWindowStart)
        {
            throw new ValidationException($"Check-in is not available yet. You can check in from {checkInWindowStart:HH:mm} (GMT+7)");
        }

        if (nowVietnam > checkInWindowEnd)
        {
            // Missed check-in - mark as no-show
            booking.Status = BookingStatus.NoShow;
            user.IncrementNoShowCount();

            // Check if user should be blocked (4 no-shows = 30 days block)
            if (user.NoShowCount >= 4)
            {
                user.BlockUser("Exceeded maximum no-show limit (4 times)", DateTime.UtcNow.AddDays(30));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            throw new ValidationException($"Check-in window has expired (until {checkInWindowEnd:HH:mm} GMT+7). This booking is marked as no-show. Total no-shows: {user.NoShowCount}/4");
        }

        // Perform check-in
        booking.CheckIn(userId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
