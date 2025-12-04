namespace CleanArchitectureTemplate.Application.Common.DTOs.Booking;

public record BookingDto(
    Guid Id,
    string BookingCode,
    Guid FacilityId,
    string FacilityName,
    string FacilityCode,
    Guid UserId,
    string UserName,
    string UserEmail,
    string UserRole,
    DateTime BookingDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string Purpose,
    int NumParticipants,
    string? EquipmentNeeded,
    string? Note,
    string Status,
    string? LecturerEmail,
    Guid? LecturerApprovedBy,
    string? LecturerApproverName,
    DateTime? LecturerApprovedAt,
    string? LecturerRejectReason,
    Guid? ApprovedBy,
    string? ApproverName,
    DateTime? ApprovedAt,
    string? RejectReason,
    DateTime? CheckedInAt,
    DateTime? CheckedOutAt,
    decimal? Rating,
    string? Comment,
    DateTime CreatedAt
);

public record CreateBookingRequest(
    Guid FacilityId,
    DateTime BookingDate,
    string StartTime,
    string EndTime,
    string Purpose,
    int NumParticipants,
    string? EquipmentNeeded,
    string? Note,
    string? LecturerEmail  // Required for Student bookings
);

public record ApproveBookingRequest(
    bool Approved,
    string? Comment
);

public record BookingListDto(
    Guid Id,
    string BookingCode,
    string FacilityName,
    string UserName,
    string UserRole,
    DateTime BookingDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string Status,
    string? LecturerEmail,
    DateTime CreatedAt
);
