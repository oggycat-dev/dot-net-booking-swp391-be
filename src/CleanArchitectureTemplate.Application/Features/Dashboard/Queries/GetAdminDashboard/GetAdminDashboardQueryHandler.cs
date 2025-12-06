using CleanArchitectureTemplate.Application.Common.DTOs.Dashboard;
using CleanArchitectureTemplate.Application.Common.Helpers;
using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Application.Features.Dashboard.Queries.GetAdminDashboard;

public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        // Use Vietnam time (GMT+7) for dashboard statistics
        var now = TimeZoneHelper.GetVietnamNow();
        var today = now.Date;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var currentTime = now.TimeOfDay;

        // User statistics
        var allUsers = await _unitOfWork.Users.GetQueryable()
            .Where(u => !u.IsDeleted)
            .ToListAsync(cancellationToken);

        var totalUsers = allUsers.Count;
        var totalStudents = allUsers.Count(u => u.Role == UserRole.Student);
        var totalLecturers = allUsers.Count(u => u.Role == UserRole.Lecturer);
        var pendingRegistrations = allUsers.Count(u => !u.IsApproved && u.Role != UserRole.Admin);

        // Campus change requests
        var pendingCampusChangeRequests = await _unitOfWork.CampusChangeRequests.GetQueryable()
            .Where(c => !c.IsDeleted && c.Status == CampusChangeRequestStatus.Pending)
            .CountAsync(cancellationToken);

        // Booking statistics
        var allBookings = await _unitOfWork.Bookings.GetQueryable()
            .Include(b => b.Facility)
            .Include(b => b.User)
            .Where(b => !b.IsDeleted)
            .ToListAsync(cancellationToken);

        var totalBookingsToday = allBookings.Count(b => b.BookingDate.Date == today);
        var totalBookingsThisWeek = allBookings.Count(b => b.BookingDate.Date >= startOfWeek && b.BookingDate.Date <= today);
        var totalBookingsThisMonth = allBookings.Count(b => b.BookingDate.Date >= startOfMonth && b.BookingDate.Date <= today);
        
        var pendingLecturerApprovals = allBookings.Count(b => b.Status == BookingStatus.WaitingLecturerApproval);
        var pendingAdminApprovals = allBookings.Count(b => b.Status == BookingStatus.Pending);
        var approvedBookingsToday = allBookings.Count(b => b.BookingDate.Date == today && b.Status == BookingStatus.Approved);
        var rejectedBookingsToday = allBookings.Count(b => b.BookingDate.Date == today && b.Status == BookingStatus.Rejected);
        
        // In-use bookings (today, currently happening)
        var inUseBookingsNow = allBookings.Count(b => 
            b.BookingDate.Date == today && 
            b.Status == BookingStatus.InUse &&
            b.StartTime <= currentTime && 
            b.EndTime >= currentTime);

        // Facility statistics
        var allFacilities = await _unitOfWork.Facilities.GetQueryable()
            .Where(f => !f.IsDeleted)
            .ToListAsync(cancellationToken);

        var totalFacilities = allFacilities.Count;
        var availableFacilities = allFacilities.Count(f => f.Status == FacilityStatus.Available && f.IsActive);
        
        // Count facilities currently in use based on active bookings
        var currentlyInUseFacilityIds = allBookings
            .Where(b => b.BookingDate.Date == today && 
                       b.Status == BookingStatus.InUse &&
                       b.StartTime <= currentTime && 
                       b.EndTime >= currentTime)
            .Select(b => b.FacilityId)
            .Distinct()
            .ToList();
        
        var inUseFacilities = currentlyInUseFacilityIds.Count;
        var maintenanceFacilities = allFacilities.Count(f => f.Status == FacilityStatus.UnderMaintenance);

        var totalCampuses = await _unitOfWork.Campuses.GetQueryable()
            .Where(c => !c.IsDeleted && c.IsActive)
            .CountAsync(cancellationToken);

        // Recent bookings (last 10)
        var recentBookings = allBookings
            .OrderByDescending(b => b.CreatedAt)
            .Take(10)
            .Select(b => new RecentBookingDto(
                b.Id,
                b.BookingCode,
                b.Facility?.FacilityName ?? string.Empty,
                b.User?.FullName ?? string.Empty,
                b.User?.Role.ToString() ?? string.Empty,
                b.BookingDate,
                b.StartTime,
                b.EndTime,
                b.Status.ToString(),
                b.CreatedAt
            ))
            .ToList();

        // Recent registrations (last 10)
        var recentRegistrations = allUsers
            .OrderByDescending(u => u.CreatedAt)
            .Take(10)
            .Select(u => new RecentUserDto(
                u.Id,
                u.UserCode,
                u.FullName,
                u.Email,
                u.Role.ToString(),
                u.IsApproved ? "Approved" : "Pending",
                u.CreatedAt
            ))
            .ToList();

        // Calculate facility utilization rate for today
        var totalPossibleSlots = totalFacilities * 14; // Assuming 14 hours per day (7 AM - 9 PM)
        var bookedSlots = allBookings
            .Where(b => b.BookingDate.Date == today && 
                       (b.Status == BookingStatus.Approved || b.Status == BookingStatus.InUse))
            .Sum(b => (b.EndTime - b.StartTime).TotalHours);
        
        var utilizationRate = totalPossibleSlots > 0 
            ? (decimal)(bookedSlots / totalPossibleSlots * 100) 
            : 0;

        return new AdminDashboardDto(
            totalUsers,
            totalStudents,
            totalLecturers,
            pendingRegistrations,
            pendingCampusChangeRequests,
            totalBookingsToday,
            totalBookingsThisWeek,
            totalBookingsThisMonth,
            pendingLecturerApprovals,
            pendingAdminApprovals,
            approvedBookingsToday,
            rejectedBookingsToday,
            inUseBookingsNow,
            totalFacilities,
            availableFacilities,
            inUseFacilities,
            maintenanceFacilities,
            totalCampuses,
            recentBookings,
            recentRegistrations,
            Math.Round(utilizationRate, 2)
        );
    }
}
