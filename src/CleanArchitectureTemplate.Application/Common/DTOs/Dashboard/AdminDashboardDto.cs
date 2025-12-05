namespace CleanArchitectureTemplate.Application.Common.DTOs.Dashboard;

/// <summary>
/// Admin dashboard overview statistics
/// </summary>
public record AdminDashboardDto(
    // User statistics
    int TotalUsers,
    int TotalStudents,
    int TotalLecturers,
    int PendingRegistrations,
    int PendingCampusChangeRequests,
    
    // Booking statistics
    int TotalBookingsToday,
    int TotalBookingsThisWeek,
    int TotalBookingsThisMonth,
    int PendingLecturerApprovals,
    int PendingAdminApprovals,
    int ApprovedBookingsToday,
    int RejectedBookingsToday,
    int InUseBookingsNow,
    
    // Facility statistics
    int TotalFacilities,
    int AvailableFacilities,
    int InUseFacilities,
    int MaintenanceFacilities,
    int TotalCampuses,
    
    // Recent activities
    List<RecentBookingDto> RecentBookings,
    List<RecentUserDto> RecentRegistrations,
    
    // Utilization rate
    decimal FacilityUtilizationRate
);

/// <summary>
/// Recent booking for dashboard
/// </summary>
public record RecentBookingDto(
    Guid Id,
    string BookingCode,
    string FacilityName,
    string UserName,
    string UserRole,
    DateTime BookingDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string Status,
    DateTime CreatedAt
);

/// <summary>
/// Recent user registration for dashboard
/// </summary>
public record RecentUserDto(
    Guid Id,
    string UserCode,
    string FullName,
    string Email,
    string Role,
    string Status,
    DateTime CreatedAt
);
