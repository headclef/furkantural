using furkantural.Application.Services.Abstract;
namespace furkantural.Infrastructure.Services.Concrete;

public class DateTimeProvider : IDateTimeProvider
{
    private static readonly TimeZoneInfo _turkeyTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

    /// <summary>
    /// Returns current time in Turkey timezone (UTC+3).
    /// </summary>
    public DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _turkeyTimeZone);

    /// <summary>
    /// Returns current UTC time.
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}