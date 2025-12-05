namespace CleanArchitectureTemplate.Application.Common.DTOs.FacilityIssue;

/// <summary>
/// DTO for facility issue report
/// </summary>
public record FacilityIssueReportDto(
    Guid Id,
    string ReportCode,
    Guid BookingId,
    string BookingCode,
    Guid FacilityId,
    string FacilityName,
    string ReportedByName,
    string ReportedByEmail,
    string IssueTitle,
    string IssueDescription,
    string Severity,
    string Category,
    List<string>? ImageUrls, // Changed to List<string> for easy display
    string Status,
    Guid? NewFacilityId,
    string? NewFacilityName,
    string? AdminResponse,
    DateTime CreatedAt,
    DateTime? HandledAt,
    DateTime? ResolvedAt
);

/// <summary>
/// Request to create a facility issue report
/// </summary>
public record CreateFacilityIssueReportRequest(
    Guid BookingId,
    string IssueTitle,
    string IssueDescription,
    string Severity, // Low, Medium, High, Critical
    string Category // Leak, Equipment, Cleanliness, Safety, Other
);

/// <summary>
/// Request to change room for a reported issue
/// </summary>
public record ChangeRoomForIssueRequest(
    Guid NewFacilityId,
    string AdminResponse
);
