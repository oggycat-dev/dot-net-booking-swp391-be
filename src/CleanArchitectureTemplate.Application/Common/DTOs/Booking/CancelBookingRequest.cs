namespace CleanArchitectureTemplate.Application.Common.DTOs.Booking;

/// <summary>
/// Request DTO for cancelling a booking
/// </summary>
public record CancelBookingRequest
{
    /// <summary>
    /// Reason for cancellation (required)
    /// </summary>
    public string Reason { get; init; } = string.Empty;
}
