namespace CleanArchitectureTemplate.Application.Common.DTOs.Facility;

public record FacilityDto(
    Guid Id,
    string FacilityCode,
    string FacilityName,
    Guid TypeId,
    string TypeName,
    Guid CampusId,
    string CampusName,
    string? Building,
    string? Floor,
    string? RoomNumber,
    int Capacity,
    string? Description,
    string? Equipment,
    string? ImageUrl,
    string Status,
    bool IsActive
);

public record CreateFacilityRequest(
    string FacilityCode,
    string FacilityName,
    Guid TypeId,
    Guid CampusId,
    string? Building,
    string? Floor,
    string? RoomNumber,
    int Capacity,
    string? Description,
    string? Equipment,
    string? ImageUrl
);

public record UpdateFacilityRequest(
    string FacilityName,
    Guid TypeId,
    string? Building,
    string? Floor,
    string? RoomNumber,
    int Capacity,
    string? Description,
    string? Equipment,
    string? ImageUrl,
    string Status,
    bool IsActive
);
