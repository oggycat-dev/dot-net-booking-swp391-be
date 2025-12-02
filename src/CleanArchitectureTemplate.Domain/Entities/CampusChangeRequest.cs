using CleanArchitectureTemplate.Domain.Commons;
using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Entities;

/// <summary>
/// Campus change request entity - stores requests from users to change their campus
/// </summary>
public class CampusChangeRequest : BaseEntity
{
    /// <summary>
    /// User who is requesting campus change
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User navigation property
    /// </summary>
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Current campus ID (before change)
    /// </summary>
    public Guid? CurrentCampusId { get; set; }
    
    /// <summary>
    /// Current campus navigation property
    /// </summary>
    public Campus? CurrentCampus { get; set; }
    
    /// <summary>
    /// Requested new campus ID
    /// </summary>
    public Guid RequestedCampusId { get; set; }
    
    /// <summary>
    /// Requested campus navigation property
    /// </summary>
    public Campus RequestedCampus { get; set; } = null!;
    
    /// <summary>
    /// Reason for campus change request
    /// </summary>
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Request status
    /// </summary>
    public CampusChangeRequestStatus Status { get; set; } = CampusChangeRequestStatus.Pending;
    
    /// <summary>
    /// Admin who reviewed this request
    /// </summary>
    public Guid? ReviewedBy { get; set; }
    
    /// <summary>
    /// Admin navigation property
    /// </summary>
    public User? ReviewedByAdmin { get; set; }
    
    /// <summary>
    /// When the request was reviewed
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
    
    /// <summary>
    /// Admin's comment/note on the request
    /// </summary>
    public string? ReviewComment { get; set; }
    
    // Domain methods
    /// <summary>
    /// Approve the campus change request
    /// </summary>
    public void Approve(Guid adminId, string? comment = null)
    {
        Status = CampusChangeRequestStatus.Approved;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
        ReviewComment = comment;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Reject the campus change request
    /// </summary>
    public void Reject(Guid adminId, string reason)
    {
        Status = CampusChangeRequestStatus.Rejected;
        ReviewedBy = adminId;
        ReviewedAt = DateTime.UtcNow;
        ReviewComment = reason;
        this.MarkAsModified();
    }
    
    /// <summary>
    /// Check if request is still pending
    /// </summary>
    public bool IsPending() => Status == CampusChangeRequestStatus.Pending;
    
    /// <summary>
    /// Check if request is approved
    /// </summary>
    public bool IsApproved() => Status == CampusChangeRequestStatus.Approved;
    
    /// <summary>
    /// Check if request is rejected
    /// </summary>
    public bool IsRejected() => Status == CampusChangeRequestStatus.Rejected;
}
