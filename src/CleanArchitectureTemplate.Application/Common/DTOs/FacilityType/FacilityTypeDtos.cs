namespace CleanArchitectureTemplate.Application.Common.DTOs.FacilityType;

public record FacilityTypeDto(
    Guid Id,
    string TypeCode,
    string TypeName,
    string? Description,
    bool IsActive
);

public record CreateFacilityTypeRequest(
    string TypeCode,
    string TypeName,
    string? Description
);

public record UpdateFacilityTypeRequest(
    string TypeName,
    string? Description,
    bool IsActive
);
