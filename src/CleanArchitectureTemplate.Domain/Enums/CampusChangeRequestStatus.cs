namespace CleanArchitectureTemplate.Domain.Enums;

/// <summary>
/// Status of a campus change request
/// </summary>
public enum CampusChangeRequestStatus
{
    /// <summary>
    /// Request is pending admin review
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Request was approved by admin
    /// </summary>
    Approved = 1,
    
    /// <summary>
    /// Request was rejected by admin
    /// </summary>
    Rejected = 2
}
