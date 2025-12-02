using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.AdminApproveBooking;

public class AdminApproveBookingCommandHandler : IRequestHandler<AdminApproveBookingCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public AdminApproveBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(AdminApproveBookingCommand request, CancellationToken cancellationToken)
    {
        // Get current admin user
        var adminId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var admin = await _unitOfWork.Users.GetByIdAsync(adminId)
            ?? throw new NotFoundException(nameof(User), adminId);

        // Verify admin role
        if (admin.Role != UserRole.Admin)
        {
            throw new ValidationException("Only admin users can approve bookings");
        }

        // Get booking
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // Verify booking is pending admin approval
        if (booking.Status != BookingStatus.Pending)
        {
            throw new ValidationException($"Booking is not pending admin approval. Current status: {booking.Status}");
        }

        // Verify booking date is not in the past
        if (booking.BookingDate.Date < DateTime.UtcNow.Date)
        {
            throw new ValidationException("Cannot approve bookings for past dates");
        }

        // Check for time conflicts before approving
        if (request.Approved)
        {
            if (await _unitOfWork.Bookings.HasConflictAsync(
                booking.FacilityId, 
                booking.BookingDate, 
                booking.StartTime, 
                booking.EndTime, 
                booking.Id))
            {
                throw new ValidationException("This time slot has been booked by another user. Please reject this booking.");
            }
        }

        var user = await _unitOfWork.Users.GetByIdAsync(booking.UserId);
        var facility = await _unitOfWork.Facilities.GetByIdAsync(booking.FacilityId);
        
        if (request.Approved)
        {
            // Admin approve
            booking.AdminApprove(adminId);
            
            // Send email notification to User about approval
            if (user != null && facility != null)
            {
                try
                {
                    var bookingDateTime = booking.BookingDate.Date.Add(booking.StartTime);
                    var endDateTime = booking.BookingDate.Date.Add(booking.EndTime);
                    await _emailService.SendBookingApprovedEmailAsync(user.Email, user.FullName, facility.FacilityName, bookingDateTime, endDateTime);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the operation
                }
            }
        }
        else
        {
            // Admin reject
            var rejectionReason = request.Comment ?? "No reason provided by admin";
            booking.AdminReject(rejectionReason);
            
            // Send email notification to User about rejection
            if (user != null && facility != null)
            {
                try
                {
                    await _emailService.SendBookingRejectedEmailAsync(user.Email, user.FullName, facility.FacilityName, rejectionReason);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the operation
                }
            }
        }

        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
