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

    public AdminApproveBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
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

        if (request.Approved)
        {
            // Admin approve
            booking.AdminApprove(adminId);
            
            // TODO: Send email notification to User about approval
            // await _emailService.SendBookingApprovedEmail(booking);
        }
        else
        {
            // Admin reject
            var rejectionReason = request.Comment ?? "No reason provided by admin";
            booking.AdminReject(rejectionReason);
            
            // TODO: Send email notification to User about rejection
            // await _emailService.SendBookingRejectedByAdminEmail(booking);
        }

        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
