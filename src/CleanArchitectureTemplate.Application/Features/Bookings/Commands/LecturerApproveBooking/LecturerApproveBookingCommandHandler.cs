using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.LecturerApproveBooking;

public class LecturerApproveBookingCommandHandler : IRequestHandler<LecturerApproveBookingCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;

    public LecturerApproveBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(LecturerApproveBookingCommand request, CancellationToken cancellationToken)
    {
        // Get current lecturer user
        var lecturerId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var lecturer = await _unitOfWork.Users.GetByIdAsync(lecturerId)
            ?? throw new NotFoundException(nameof(User), lecturerId);

        // Verify lecturer role
        if (lecturer.Role != UserRole.Lecturer)
        {
            throw new ValidationException("Only lecturers can approve student bookings");
        }

        // Get booking
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // Verify booking is waiting for lecturer approval
        if (booking.Status != BookingStatus.WaitingLecturerApproval)
        {
            throw new ValidationException($"Booking is not waiting for lecturer approval. Current status: {booking.Status}");
        }

        // Verify this lecturer is the one assigned to this booking
        if (booking.LecturerEmail?.ToLower() != lecturer.Email.ToLower())
        {
            throw new ValidationException("You are not the assigned lecturer for this booking");
        }

        // Verify booking date is not in the past
        if (booking.BookingDate.Date < DateTime.UtcNow.Date)
        {
            throw new ValidationException("Cannot approve bookings for past dates");
        }

        if (request.Approved)
        {
            // Approve - move to Pending for Admin
            booking.LecturerApprove(lecturerId);
            
            // Note: Email to admin is not implemented as admin will see it in pending list
        }
        else
        {
            // Reject
            var rejectionReason = request.Comment ?? "No reason provided by lecturer";
            booking.LecturerReject(rejectionReason);
            
            // Send email notification to Student about rejection
            var student = await _unitOfWork.Users.GetByIdAsync(booking.UserId);
            var facility = await _unitOfWork.Facilities.GetByIdAsync(booking.FacilityId);
            if (student != null && facility != null)
            {
                try
                {
                    await _emailService.SendBookingRejectedEmailAsync(student.Email, student.FullName, facility.FacilityName, rejectionReason);
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
