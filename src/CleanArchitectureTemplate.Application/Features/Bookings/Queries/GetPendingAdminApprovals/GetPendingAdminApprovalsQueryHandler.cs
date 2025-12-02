using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetPendingAdminApprovals;

public class GetPendingAdminApprovalsQueryHandler : IRequestHandler<GetPendingAdminApprovalsQuery, List<BookingListDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingAdminApprovalsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BookingListDto>> Handle(GetPendingAdminApprovalsQuery request, CancellationToken cancellationToken)
    {
        var pendingBookings = await _unitOfWork.Bookings.GetPendingAdminApprovalsAsync();

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
