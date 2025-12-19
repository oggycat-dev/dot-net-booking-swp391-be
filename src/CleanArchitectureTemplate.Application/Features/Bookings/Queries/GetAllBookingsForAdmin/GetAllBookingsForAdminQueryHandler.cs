using AutoMapper;
using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Common.Models;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetAllBookingsForAdmin;

/// <summary>
/// Handler for GetAllBookingsForAdminQuery
/// </summary>
public class GetAllBookingsForAdminQueryHandler : IRequestHandler<GetAllBookingsForAdminQuery, PaginatedResult<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBookingsForAdminQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<BookingDto>> Handle(GetAllBookingsForAdminQuery request, CancellationToken cancellationToken)
    {
        // Get all bookings with filters applied in repository
        var allBookings = await _unitOfWork.Bookings.GetAllBookingsForAdminAsync(
            request.FacilityId,
            request.CampusId,
            request.FromDate,
            request.ToDate,
            request.Status,
            request.SearchTerm
        );

        // Get total count
        var totalCount = allBookings.Count;

        // Apply pagination
        var paginatedBookings = allBookings
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Map to DTOs
        var bookingDtos = paginatedBookings.Select(b => _mapper.Map<BookingDto>(b)).ToList();

        return new PaginatedResult<BookingDto>
        {
            Items = bookingDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
