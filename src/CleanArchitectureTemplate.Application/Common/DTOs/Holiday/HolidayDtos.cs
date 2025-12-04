namespace CleanArchitectureTemplate.Application.Common.DTOs.Holiday;

public record HolidayDto(
    Guid Id,
    string HolidayName,
    DateTime HolidayDate,
    bool IsRecurring,
    string? Description,
    DateTime CreatedAt
);

public record CreateHolidayRequest(
    string HolidayName,
    DateTime HolidayDate,
    bool IsRecurring,
    string? Description
);
