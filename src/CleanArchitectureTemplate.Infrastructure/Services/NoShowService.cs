using CleanArchitectureTemplate.Application.Common.Interfaces;
using CleanArchitectureTemplate.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Infrastructure.Services;

public class NoShowService : INoShowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NoShowService> _logger;

    public NoShowService(IUnitOfWork unitOfWork, ILogger<NoShowService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task CheckAndMarkNoShowBookingsAsync(Guid userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return;

            // Get all approved bookings of this user
            var userBookings = await _unitOfWork.Bookings.GetByUserIdAsync(userId);
            var approvedBookings = userBookings.Where(b => b.Status == BookingStatus.Approved).ToList();

            if (!approvedBookings.Any()) return;

            var now = DateTime.UtcNow;
            var noShowBookings = new List<Guid>();

            foreach (var booking in approvedBookings)
            {
                var bookingEndDateTime = DateTime.SpecifyKind(booking.BookingDate.Date, DateTimeKind.Utc)
                    .Add(booking.EndTime);
                var checkOutDeadline = bookingEndDateTime.AddMinutes(15);

                // If checkout deadline has passed and user hasn't checked out
                if (now > checkOutDeadline && !booking.CheckedOutAt.HasValue)
                {
                    // Mark as no-show
                    booking.Status = BookingStatus.NoShow;
                    noShowBookings.Add(booking.Id);
                    
                    _logger.LogInformation(
                        "Booking {BookingId} marked as NoShow for user {UserId}. Deadline: {Deadline}, Current: {Now}",
                        booking.Id, userId, checkOutDeadline, now);
                }
            }

            if (noShowBookings.Any())
            {
                // Increment no-show count for each no-show booking
                foreach (var _ in noShowBookings)
                {
                    user.IncrementNoShowCount();
                }

                // Check if user should be blocked (>= 4 no-shows)
                if (user.NoShowCount >= 4)
                {
                    user.BlockUser($"Exceeded maximum no-show limit ({user.NoShowCount} times)", DateTime.UtcNow.AddDays(30));
                    
                    _logger.LogWarning(
                        "User {UserId} blocked for 30 days due to {NoShowCount} no-shows",
                        userId, user.NoShowCount);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Processed {Count} no-show bookings for user {UserId}. Total no-shows: {NoShowCount}",
                    noShowBookings.Count, userId, user.NoShowCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking no-show bookings for user {UserId}", userId);
            // Don't throw - this is a background check, shouldn't block user login
        }
    }
}
