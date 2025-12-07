namespace CleanArchitectureTemplate.Application.Common.DTOs.Notifications;

public record NotificationDto(
    Guid Id,
    string Title,
    string Body,
    string Type,
    Guid? RelatedEntityId,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt,
    string? Data
);

public record NotificationSummaryDto(
    int TotalCount,
    int UnreadCount,
    List<NotificationDto> Notifications
);
