namespace CleanArchitectureTemplate.Application.Common.Helpers;

/// <summary>
/// Helper class for handling timezone conversions
/// </summary>
public static class TimeZoneHelper
{
    /// <summary>
    /// SE Asia Standard Time (GMT+7) - Vietnam, Thailand, Indonesia (Western)
    /// This timezone ID works on both Windows and Linux
    /// </summary>
    private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

    /// <summary>
    /// Get current Vietnam time (GMT+7)
    /// </summary>
    public static DateTime GetVietnamNow()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);
    }

    /// <summary>
    /// Convert UTC datetime to Vietnam time (GMT+7)
    /// </summary>
    public static DateTime ConvertToVietnamTime(DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, VietnamTimeZone);
    }

    /// <summary>
    /// Convert Vietnam time to UTC
    /// </summary>
    public static DateTime ConvertToUtc(DateTime vietnamDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, VietnamTimeZone);
    }

    /// <summary>
    /// Combine booking date and time, treating them as Vietnam time, then convert to UTC
    /// </summary>
    public static DateTime CombineBookingDateTimeAsUtc(DateTime bookingDate, TimeSpan bookingTime)
    {
        // Treat the booking date + time as Vietnam time
        var vietnamDateTime = bookingDate.Date.Add(bookingTime);
        
        // Specify as Unspecified kind to allow conversion
        vietnamDateTime = DateTime.SpecifyKind(vietnamDateTime, DateTimeKind.Unspecified);
        
        // Convert to UTC for storage/comparison
        return TimeZoneInfo.ConvertTimeToUtc(vietnamDateTime, VietnamTimeZone);
    }

    /// <summary>
    /// Get timezone offset hours (for display purposes)
    /// </summary>
    public static double GetVietnamOffsetHours()
    {
        return VietnamTimeZone.GetUtcOffset(DateTime.UtcNow).TotalHours;
    }
}
