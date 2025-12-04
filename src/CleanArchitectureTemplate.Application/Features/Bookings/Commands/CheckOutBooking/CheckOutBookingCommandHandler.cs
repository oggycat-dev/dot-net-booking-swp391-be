using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CheckOutBooking;

public class CheckOutBookingCommandHandler : IRequestHandler<CheckOutBookingCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CheckOutBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CheckOutBookingCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new ValidationException("User not authenticated");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // Only booking owner can check-out
        if (booking.UserId != userId)
        {
            throw new ValidationException("You can only check-out your own bookings");
        }

        // Check if checked in
        if (!booking.CheckedInAt.HasValue)
        {
            throw new ValidationException("Cannot check-out without checking in first");
        }

        // Check if already checked out
        if (booking.CheckedOutAt.HasValue)
        {
            throw new ValidationException("This booking has already been checked out");
        }

        var now = DateTime.UtcNow;
        var bookingEndDateTime = DateTime.SpecifyKind(booking.BookingDate.Date, DateTimeKind.Utc).Add(booking.EndTime);
        
        // Check-out window: from end time to 15 minutes after end time
        var checkOutWindowStart = bookingEndDateTime;
        var checkOutWindowEnd = bookingEndDateTime.AddMinutes(15);

        if (now < checkOutWindowStart)
        {
            throw new ValidationException($"Check-out is not available yet. You can check out from {checkOutWindowStart:HH:mm}");
        }

        if (now > checkOutWindowEnd)
        {
            // Missed check-out - mark as no-show
            booking.Status = BookingStatus.NoShow;
            user.IncrementNoShowCount();

            // Check if user should be blocked (4 no-shows = 30 days block)
            if (user.NoShowCount >= 4)
            {
                user.BlockUser("Exceeded maximum no-show limit (4 times)", DateTime.UtcNow.AddDays(30));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            throw new ValidationException($"Check-out window has expired (until {checkOutWindowEnd:HH:mm}). This booking is marked as no-show. Total no-shows: {user.NoShowCount}/4");
        }

        // Perform check-out
        booking.CheckOut(userId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
