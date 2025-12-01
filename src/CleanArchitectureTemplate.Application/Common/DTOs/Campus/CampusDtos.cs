namespace CleanArchitectureTemplate.Application.Common.DTOs.Campus;

public record CampusDto(
    Guid Id,
    string CampusCode,
    string CampusName,
    string Address,
    TimeSpan WorkingHoursStart,
    TimeSpan WorkingHoursEnd,
    string? ContactPhone,
    string? ContactEmail,
    bool IsActive
);

public record CreateCampusRequest(
    string CampusCode,
    string CampusName,
    string Address,
    TimeSpan WorkingHoursStart,
    TimeSpan WorkingHoursEnd,
    string? ContactPhone,
    string? ContactEmail
);

public record UpdateCampusRequest(
    string CampusName,
    string Address,
    TimeSpan WorkingHoursStart,
    TimeSpan WorkingHoursEnd,
    string? ContactPhone,
    string? ContactEmail,
    bool IsActive
);
