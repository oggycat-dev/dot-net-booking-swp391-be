using CleanArchitectureTemplate.Application.Common.DTOs.Booking;
using MediatR;

namespace CleanArchitectureTemplate.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand : IRequest<BookingDto>
{
    public Guid FacilityId { get; init; }
    public DateTime BookingDate { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public int NumParticipants { get; init; }
    public string? EquipmentNeeded { get; init; }
    public string? Note { get; init; }
    public string? LecturerEmail { get; init; }  // Required for Student bookings
}
