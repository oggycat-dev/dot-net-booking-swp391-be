namespace CleanArchitectureTemplate.Application.Common.DTOs.CampusChangeRequest;

public record CampusChangeRequestDto(
    Guid Id,
    Guid UserId,
    string UserName,
    string UserEmail,
    Guid? CurrentCampusId,
    string? CurrentCampusName,
    Guid RequestedCampusId,
    string RequestedCampusName,
    string Reason,
    string Status,
    Guid? ReviewedBy,
    string? ReviewedByName,
    DateTime? ReviewedAt,
    string? ReviewComment,
    DateTime CreatedAt
);

public record RequestCampusChangeRequest(
    Guid RequestedCampusId,
    string Reason
);

public record ApproveCampusChangeRequest(
    bool Approved,
    string? Comment
);

public record MyCampusChangeRequestDto(
    Guid Id,
    Guid? CurrentCampusId,
    string? CurrentCampusName,
    Guid RequestedCampusId,
    string RequestedCampusName,
    string Reason,
    string Status,
    string? ReviewComment,
    DateTime CreatedAt,
    DateTime? ReviewedAt
);
