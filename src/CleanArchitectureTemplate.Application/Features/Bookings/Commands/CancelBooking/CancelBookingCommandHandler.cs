using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CancelBooking;

/// <summary>
/// Handler for CancelBookingCommand - Admin cancels an approved booking
/// </summary>
public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public CancelBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        // Get current admin user
        var adminId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var admin = await _unitOfWork.Users.GetByIdAsync(adminId)
            ?? throw new NotFoundException(nameof(User), adminId);

        // Verify admin role
        if (admin.Role != UserRole.Admin)
        {
            throw new ValidationException("Only admin users can cancel bookings");
        }

        // Get booking with related data
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // Verify booking can be cancelled
        if (booking.Status == BookingStatus.Completed)
        {
            throw new ValidationException("Cannot cancel completed booking");
        }
        
        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new ValidationException("Booking is already cancelled");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(booking.UserId);
        var facility = await _unitOfWork.Facilities.GetByIdAsync(booking.FacilityId);

        // Cancel the booking using domain method
        booking.AdminCancel(request.Reason);

        // Send email notification to user who created the booking
        if (user != null && facility != null)
        {
            try
            {
                var bookingDateTime = booking.BookingDate.Date.Add(booking.StartTime);
                var endDateTime = booking.BookingDate.Date.Add(booking.EndTime);
                await _emailService.SendBookingCancelledByAdminEmailAsync(
                    user.Email, 
                    user.FullName, 
                    facility.FacilityName, 
                    bookingDateTime, 
                    endDateTime,
                    request.Reason,
                    admin.FullName
                );
            }
            catch
            {
                // Log error but don't fail the operation
            }
        }

        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
