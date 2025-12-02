using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Exceptions;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetPendingLecturerApprovals;

public class GetPendingLecturerApprovalsQueryHandler : IRequestHandler<GetPendingLecturerApprovalsQuery, List<BookingListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetPendingLecturerApprovalsQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<List<BookingListDto>> Handle(GetPendingLecturerApprovalsQuery request, CancellationToken cancellationToken)
    {
        var lecturerId = _currentUserService.UserId 
            ?? throw new ValidationException("User not authenticated");

        var lecturer = await _unitOfWork.Users.GetByIdAsync(lecturerId)
            ?? throw new NotFoundException(nameof(User), lecturerId);

        if (lecturer.Role != UserRole.Lecturer)
        {
            throw new ValidationException("Only lecturers can view lecturer approval requests");
        }

        var pendingBookings = await _unitOfWork.Bookings
            .GetWaitingLecturerApprovalByEmailAsync(lecturer.Email);

        return pendingBookings.Select(b => new BookingListDto(
            b.Id,
            b.BookingCode,
            b.Facility.FacilityName,
            b.User.FullName,
            b.User.Role.ToString(),
            b.BookingDate,
            b.StartTime,
            b.EndTime,
            b.Status.ToString(),
            b.LecturerEmail,
            b.CreatedAt
        )).ToList();
    }
}
