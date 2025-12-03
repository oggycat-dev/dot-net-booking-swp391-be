using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetMyBookingHistory;

public class GetMyBookingHistoryQueryHandler : IRequestHandler<GetMyBookingHistoryQuery, List<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyBookingHistoryQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<List<BookingDto>> Handle(GetMyBookingHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new ValidationException("User not authenticated");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        if (user.Role != UserRole.Student && user.Role != UserRole.Lecturer)
        {
            throw new ValidationException("Only Students and Lecturers can access this endpoint");
        }

        // Get all bookings created by this user
        var bookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);

        return bookings.Select(b => new BookingDto(
            b.Id,
            b.BookingCode,
            b.Facility.Id,
            b.Facility.FacilityName,
            b.Facility.FacilityCode,
            b.User.Id,
            b.User.FullName,
            b.User.Email,
            b.User.Role.ToString(),
            b.BookingDate,
            b.StartTime,
            b.EndTime,
            b.Purpose,
            b.NumParticipants,
            b.EquipmentNeeded,
            b.Note,
            b.Status.ToString(),
            b.LecturerEmail,
            b.LecturerApprovedBy,
            b.LecturerApprover?.FullName,
            b.LecturerApprovedAt,
            b.LecturerRejectReason,
            b.ApprovedBy,
            b.Approver?.FullName,
            b.ApprovedAt,
            b.RejectReason,
            b.CheckedInAt,
            b.CheckedOutAt,
            b.Rating,
            b.Comment,
            b.CreatedAt
        )).ToList();
    }
}
