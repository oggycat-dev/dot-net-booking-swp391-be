namespace CleanArchitectureTemplate.Domain.Enums;

/// <summary>
/// User roles enumeration for FPT Booking System
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Student role - can book facilities with 7 days advance limit
    /// </summary>
    Student = 0,
    
    /// <summary>
    /// Lecturer role - can book facilities with 30 days advance limit and recurring bookings
    /// </summary>
    Lecturer = 1,
    
    /// <summary>
    /// Facility Administrator - manages facilities, bookings, and users
    /// </summary>
    Admin = 2
}

/// <summary>
/// Booking status enumeration
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking request submitted, waiting for approval
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Booking approved by admin
    /// </summary>
    Approved = 1,
    
    /// <summary>
    /// Booking rejected by admin
    /// </summary>
    Rejected = 2,
    
    /// <summary>
    /// User confirmed the approved booking
    /// </summary>
    Confirmed = 3,
    
    /// <summary>
    /// Currently in use (checked in)
    /// </summary>
    InUse = 4,
    
    /// <summary>
    /// Booking completed successfully
    /// </summary>
    Completed = 5,
    
    /// <summary>
    /// Booking cancelled by user or admin
    /// </summary>
    Cancelled = 6,
    
    /// <summary>
    /// User did not check in within 15 minutes
    /// </summary>
    NoShow = 7
}

/// <summary>
/// Facility status enumeration
/// </summary>
public enum FacilityStatus
{
    /// <summary>
    /// Facility is available for booking
    /// </summary>
    Available = 0,
    
    /// <summary>
    /// Facility is under maintenance
    /// </summary>
    UnderMaintenance = 1,
    
    /// <summary>
    /// Facility is temporarily unavailable
    /// </summary>
    Unavailable = 2
}

/// <summary>
/// Maintenance status enumeration
/// </summary>
public enum MaintenanceStatus
{
    /// <summary>
    /// Maintenance scheduled but not started
    /// </summary>
    Scheduled = 0,
    
    /// <summary>
    /// Maintenance currently in progress
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Maintenance completed
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Maintenance cancelled
    /// </summary>
    Cancelled = 3
}

/// <summary>
/// Booking conflict type enumeration
/// </summary>
public enum ConflictType
{
    /// <summary>
    /// Two bookings overlap in time
    /// </summary>
    TimeOverlap = 0,
    
    /// <summary>
    /// Two approved bookings for same facility and time
    /// </summary>
    DoubleBooking = 1,
    
    /// <summary>
    /// Booking conflicts with maintenance schedule
    /// </summary>
    MaintenanceConflict = 2
}

/// <summary>
/// Conflict resolution method enumeration
/// </summary>
public enum ResolutionMethod
{
    /// <summary>
    /// Resolved by system priority rules (Lecturer > Student, Earlier > Later)
    /// </summary>
    PriorityRule = 0,
    
    /// <summary>
    /// Manually resolved by admin
    /// </summary>
    ManualResolution = 1,
    
    /// <summary>
    /// One booking rescheduled to different time/facility
    /// </summary>
    Reschedule = 2,
    
    /// <summary>
    /// One booking cancelled
    /// </summary>
    Cancellation = 3
}
