using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// User entity for FPT Booking System (Student, Lecturer, Admin)
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User code - Student ID or Employee ID (unique identifier)
    /// </summary>
    public string UserCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email address (must be @fpt.edu.vn domain)
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// User's hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// User's role (Student, Lecturer, Admin)
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Student;
    
    /// <summary>
    /// Department (e.g., Computer Science, Business Administration)
    /// </summary>
    public string? Department { get; set; }
    
    /// <summary>
    /// Major - for students only (e.g., Software Engineering, AI)
    /// </summary>
    public string? Major { get; set; }
    
    /// <summary>
    /// Campus ID - which campus the user belongs to
    /// </summary>
    public Guid? CampusId { get; set; }
    
    /// <summary>
    /// Campus navigation property
    /// </summary>
    public Campus? Campus { get; set; }
    
    /// <summary>
    /// Count of no-shows (did not check-in within 15 minutes)
    /// </summary>
    public int NoShowCount { get; set; } = 0;
    
    /// <summary>
    /// Whether the user account is blocked
    /// </summary>
    public bool IsBlocked { get; set; } = false;
    
    /// <summary>
    /// Date until which the user is blocked
    /// </summary>
    public DateTime? BlockedUntil { get; set; }
    
    /// <summary>
    /// Reason for blocking the user
    /// </summary>
    public string? BlockedReason { get; set; }
    
    /// <summary>
    /// Password reset verification code
    /// </summary>
    public string? PasswordResetCode { get; set; }
    
    /// <summary>
    /// Password reset code expiry time
    /// </summary>
    public DateTime? PasswordResetCodeExpiry { get; set; }
    
    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Whether the user registration is approved by admin (for Student/Lecturer only)
    /// </summary>
    public bool IsApproved { get; set; } = false;
    
    /// <summary>
    /// Admin who approved this registration
    /// </summary>
    public Guid? ApprovedBy { get; set; }
    
    /// <summary>
    /// Admin navigation property
    /// </summary>
    public User? ApprovedByAdmin { get; set; }
    
    /// <summary>
    /// When the registration was approved
    /// </summary>
    public DateTime? ApprovedAt { get; set; }
    
    /// <summary>
    /// Whether the user's email is confirmed
    /// </summary>
    public bool EmailConfirmed { get; set; }
    
    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLogin { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Bookings created by this user
    /// </summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    
    /// <summary>
    /// Bookings approved by this user (for admins)
    /// </summary>
    public ICollection<Booking> ApprovedBookings { get; set; } = new List<Booking>();
    
    // Domain methods
    /// <summary>
    /// Record a no-show incident
    /// </summary>
    public void RecordNoShow()
    {
        NoShowCount++;
        
        // Auto-block after 4 no-shows for 30 days
        if (NoShowCount >= 4)
        {
            BlockUser("4 no-shows reached - automatic 30 days block", DateTime.UtcNow.AddDays(30));
        }
        
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Increment no-show count (alias for RecordNoShow)
    /// </summary>
    public void IncrementNoShowCount()
    {
        RecordNoShow();
    }
    
    /// <summary>
    /// Block user account
    /// </summary>
    public void BlockUser(string reason, DateTime until)
    {
        IsBlocked = true;
        BlockedReason = reason;
        BlockedUntil = until;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Unblock user account
    /// </summary>
    public void UnblockUser()
    {
        IsBlocked = false;
        BlockedReason = null;
        BlockedUntil = null;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Check if user is currently blocked
    /// </summary>
    public bool IsCurrentlyBlocked()
    {
        if (!IsBlocked) return false;
        
        // Auto-unblock if block period expired
        if (BlockedUntil.HasValue && BlockedUntil.Value < DateTime.UtcNow)
        {
            UnblockUser();
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Reset no-show count (used when unblocking)
    /// </summary>
    public void ResetNoShowCount()
    {
        NoShowCount = 0;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Domain method to activate user account
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Domain method to deactivate user account
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Domain method to confirm email
    /// </summary>
    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Update last login timestamp
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Check if user can book (not blocked, account active, email confirmed)
    /// </summary>
    public bool CanBook()
    {
        return IsActive && EmailConfirmed && !IsCurrentlyBlocked();
    }
    
    /// <summary>
    /// Get max days ahead user can book based on role
    /// </summary>
    public int GetMaxBookingDaysAhead()
    {
        return Role switch
        {
            UserRole.Student => 7,
            UserRole.Lecturer => 30,
            UserRole.Admin => 365,
            _ => 7
        };
    }
}
