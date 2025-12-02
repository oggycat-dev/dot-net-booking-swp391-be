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

    public LecturerApproveBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
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
            
            // TODO: Send email notification to Admin about new pending booking
            // await _emailService.SendBookingPendingAdminApprovalEmail(booking);
        }
        else
        {
            // Reject
            var rejectionReason = request.Comment ?? "No reason provided by lecturer";
            booking.LecturerReject(rejectionReason);
            
            // TODO: Send email notification to Student about rejection
            // await _emailService.SendBookingRejectedByLecturerEmail(booking);
        }

        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
