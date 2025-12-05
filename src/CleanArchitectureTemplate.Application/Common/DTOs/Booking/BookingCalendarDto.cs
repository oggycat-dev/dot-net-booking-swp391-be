namespace CleanArchitectureTemplate.Application.Common.DTOs.Booking;

/// <summary>
/// DTO for calendar view - showing booked time slots
/// </summary>
public record BookingCalendarDto(
    Guid Id,
    string BookingCode,
    Guid FacilityId,
    string FacilityName,
    string FacilityCode,
    string CampusName,
    string UserName,
    string UserRole,
    DateTime BookingDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string Status,
    string Purpose,
    int NumParticipants
);
