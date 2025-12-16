using AutoMapper;
using CleanArchitectureTemplate.Application.Common.DTOs;
using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Application.Common.Models;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Queries.GetApprovedBookings;

/// <summary>
/// Handler for GetApprovedBookingsQuery
/// </summary>
public class GetApprovedBookingsQueryHandler : IRequestHandler<GetApprovedBookingsQuery, PaginatedResult<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetApprovedBookingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<BookingDto>> Handle(GetApprovedBookingsQuery request, CancellationToken cancellationToken)
    {
        // Get all approved bookings with filters applied in repository
        var allBookings = await _unitOfWork.Bookings.GetApprovedBookingsAsync(
            request.FacilityId,
            request.CampusId,
            request.FromDate,
            request.ToDate,
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
