using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Booking entity representing facility booking requests and reservations
/// </summary>
public class Booking : BaseEntity
{
    /// <summary>
    /// Booking code (unique identifier, format: BK-YYYYMMDD-XXXX)
    /// </summary>
    public string BookingCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Foreign key to Facility
    /// </summary>
    public Guid FacilityId { get; set; }
    
    /// <summary>
    /// Facility navigation property
    /// </summary>
    public Facility Facility { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to User (booking requester)
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User navigation property (booking requester)
    /// </summary>
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Booking date
    /// </summary>
    public DateTime BookingDate { get; set; }
    
    /// <summary>
    /// Start time of the booking
    /// </summary>
    public TimeSpan StartTime { get; set; }
    
    /// <summary>
    /// End time of the booking
    /// </summary>
    public TimeSpan EndTime { get; set; }
    
    /// <summary>
    /// Purpose of the booking (required)
    /// </summary>
    public string Purpose { get; set; } = string.Empty;
    
    /// <summary>
    /// Number of participants
    /// </summary>
    public int NumParticipants { get; set; }
    
    /// <summary>
    /// Equipment needed (JSON or comma-separated list)
    /// </summary>
    public string? EquipmentNeeded { get; set; }
    
    /// <summary>
    /// Additional notes from the user
    /// </summary>
    public string? Note { get; set; }
    
    /// <summary>
    /// Current status of the booking
    /// </summary>
    public BookingStatus Status { get; set; } = BookingStatus.WaitingLecturerApproval;
    
    // Lecturer approval fields (for Student bookings only)
    /// <summary>
    /// Lecturer email for Student bookings (required for students)
    /// </summary>
    public string? LecturerEmail { get; set; }
    
    /// <summary>
    /// Foreign key to Lecturer who approved the student booking
    /// </summary>
    public Guid? LecturerApprovedBy { get; set; }
    
    /// <summary>
    /// Lecturer who approved the booking navigation property
    /// </summary>
    public User? LecturerApprover { get; set; }
    
    /// <summary>
    /// Timestamp when lecturer approved the booking
    /// </summary>
    public DateTime? LecturerApprovedAt { get; set; }
    
    /// <summary>
    /// Reason for lecturer rejection (if rejected by lecturer)
    /// </summary>
    public string? LecturerRejectReason { get; set; }
    
    // Admin approval fields
    /// <summary>
    /// Foreign key to User who approved the booking (admin)
    /// </summary>
    public Guid? ApprovedBy { get; set; }
    
    /// <summary>
    /// User who approved the booking navigation property
    /// </summary>
    public User? Approver { get; set; }
    
    /// <summary>
    /// Timestamp when booking was approved by admin
    /// </summary>
    public DateTime? ApprovedAt { get; set; }
    
    /// <summary>
    /// Reason for rejection (if status is Rejected)
    /// </summary>
    public string? RejectReason { get; set; }
    
    // Check-in/Check-out fields
    /// <summary>
    /// Timestamp when user checked in
    /// </summary>
    public DateTime? CheckedInAt { get; set; }
    
    /// <summary>
    /// Foreign key to User who performed check-in (usually admin/security)
    /// </summary>
    public Guid? CheckedInBy { get; set; }
    
    /// <summary>
    /// User who performed check-in navigation property
    /// </summary>
    public User? CheckInPerformer { get; set; }
    
    /// <summary>
    /// Timestamp when user checked out
    /// </summary>
    public DateTime? CheckedOutAt { get; set; }
    
    /// <summary>
    /// Foreign key to User who performed check-out (usually admin/security)
    /// </summary>
    public Guid? CheckedOutBy { get; set; }
    
    /// <summary>
    /// User who performed check-out navigation property
    /// </summary>
    public User? CheckOutPerformer { get; set; }
    
    // Feedback fields
    /// <summary>
    /// Rating given by user (1.0 to 5.0)
    /// </summary>
    public decimal? Rating { get; set; }
    
    /// <summary>
    /// Comment/review from user
    /// </summary>
    public string? Comment { get; set; }
    
    // Cancellation fields
    /// <summary>
    /// Timestamp when booking was cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }
    
    /// <summary>
    /// Reason for cancellation
    /// </summary>
    public string? CancellationReason { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Booking history records
    /// </summary>
    public ICollection<BookingHistory> History { get; set; } = new List<BookingHistory>();
    
    /// <summary>
    /// Conflicts involving this booking
    /// </summary>
    public ICollection<BookingConflict> ConflictsAsFirst { get; set; } = new List<BookingConflict>();
    
    /// <summary>
    /// Conflicts involving this booking
    /// </summary>
    public ICollection<BookingConflict> ConflictsAsSecond { get; set; } = new List<BookingConflict>();
    
    // Domain methods
    /// <summary>
    /// Generate booking code (format: BK-YYYYMMDD-XXXX)
    /// </summary>
    public static string GenerateBookingCode(DateTime bookingDate)
    {
        var dateStr = bookingDate.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"BK-{dateStr}-{random}";
    }
    
    /// <summary>
    /// Get booking datetime (date + start time)
    /// </summary>
    public DateTime GetStartDateTime()
    {
        return BookingDate.Date.Add(StartTime);
    }
    
    /// <summary>
    /// Get booking end datetime (date + end time)
    /// </summary>
    public DateTime GetEndDateTime()
    {
        return BookingDate.Date.Add(EndTime);
    }
    
    /// <summary>
    /// Get duration in minutes
    /// </summary>
    public int GetDurationMinutes()
    {
        return (int)(EndTime - StartTime).TotalMinutes;
    }
    
    /// <summary>
    /// Lecturer approve booking (for Student bookings)
    /// </summary>
    public void LecturerApprove(Guid lecturerUserId)
    {
        Status = BookingStatus.Pending; // Move to Pending for Admin review
        LecturerApprovedBy = lecturerUserId;
        LecturerApprovedAt = DateTime.UtcNow;
        LecturerRejectReason = null;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Lecturer reject booking (for Student bookings)
    /// </summary>
    public void LecturerReject(string reason)
    {
        Status = BookingStatus.Rejected;
        LecturerRejectReason = reason;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Admin approve booking
    /// </summary>
    public void AdminApprove(Guid adminUserId)
    {
        Status = BookingStatus.Approved;
        ApprovedBy = adminUserId;
        ApprovedAt = DateTime.UtcNow;
        RejectReason = null;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Admin reject booking
    /// </summary>
    public void AdminReject(string reason)
    {
        Status = BookingStatus.Rejected;
        RejectReason = reason;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Check in
    /// </summary>
    public void CheckIn(Guid? checkedInByUserId = null)
    {
        Status = BookingStatus.InUse;
        CheckedInAt = DateTime.UtcNow;
        CheckedInBy = checkedInByUserId;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Check out
    /// </summary>
    public void CheckOut(Guid? checkedOutByUserId = null)
    {
        Status = BookingStatus.Completed;
        CheckedOutAt = DateTime.UtcNow;
        CheckedOutBy = checkedOutByUserId;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Mark as no-show
    /// </summary>
    public void MarkAsNoShow()
    {
        Status = BookingStatus.NoShow;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Cancel booking
    /// </summary>
    public void Cancel(string reason)
    {
        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Admin cancel booking (can cancel approved bookings)
    /// </summary>
    public void AdminCancel(string reason)
    {
        if (Status == BookingStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel completed booking");
        }
        
        if (Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException("Booking is already cancelled");
        }
        
        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Add rating and comment
    /// </summary>
    public void AddFeedback(decimal rating, string? comment = null)
    {
        if (rating < 1.0m || rating > 5.0m)
            throw new ArgumentException("Rating must be between 1.0 and 5.0");
            
        Rating = rating;
        Comment = comment;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Check if booking is in the past
    /// </summary>
    public bool IsPast()
    {
        return GetEndDateTime() < DateTime.UtcNow;
    }
    
    /// <summary>
    /// Check if booking is upcoming (within next 7 days)
    /// </summary>
    public bool IsUpcoming()
    {
        var now = DateTime.UtcNow;
        var startDateTime = GetStartDateTime();
        return startDateTime > now && startDateTime <= now.AddDays(7);
    }
    
    /// <summary>
    /// Check if booking is active (approved or in use)
    /// </summary>
    public bool IsActive()
    {
        return Status == BookingStatus.Approved || Status == BookingStatus.InUse;
    }
    
    /// <summary>
    /// Check if user can check in (within 15 minutes before start time)
    /// </summary>
    public bool CanCheckIn()
    {
        if (Status != BookingStatus.Approved)
            return false;
            
        var now = DateTime.UtcNow;
        var startDateTime = GetStartDateTime();
        
        // Can check in up to 15 minutes before and up to 15 minutes after start time
        return now >= startDateTime.AddMinutes(-15) && now <= startDateTime.AddMinutes(15);
    }
    
    /// <summary>
    /// Check if booking should be auto no-show (15 minutes after start time)
    /// </summary>
    public bool ShouldBeMarkedNoShow()
    {
        if (Status != BookingStatus.Approved)
            return false;
            
        var now = DateTime.UtcNow;
        var startDateTime = GetStartDateTime();
        
        return now > startDateTime.AddMinutes(15);
    }
    
    /// <summary>
    /// Check if bookings overlap in time
    /// </summary>
    public bool OverlapsWith(Booking other)
    {
        if (BookingDate.Date != other.BookingDate.Date)
            return false;
            
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }
    
    /// <summary>
    /// Check if user can cancel (more than 2 hours before start)
    /// </summary>
    public bool CanCancelWithoutPenalty()
    {
        var now = DateTime.UtcNow;
        var startDateTime = GetStartDateTime();
        
        return now < startDateTime.AddHours(-2);
    }
}
