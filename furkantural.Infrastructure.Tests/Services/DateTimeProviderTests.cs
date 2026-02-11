using FluentAssertions;
using furkantural.Infrastructure.Services.Concrete;
namespace furkantural.Infrastructure.Tests.Services;

public class DateTimeProviderTests
{
    private readonly DateTimeProvider _provider = new();

    [Fact]
    public void Now_ShouldReturnTurkeyTime()
    {
        var now = _provider.Now;
        var expectedUtcOffset = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time").GetUtcOffset(DateTime.UtcNow);

        // Turkey is always UTC+3
        expectedUtcOffset.Should().Be(TimeSpan.FromHours(3));

        // The provider's Now should be approximately UtcNow + 3 hours
        var diff = now - DateTime.UtcNow;
        diff.TotalHours.Should().BeApproximately(3, 0.1);
    }

    [Fact]
    public void UtcNow_ShouldBeCloseToDateTimeUtcNow()
    {
        var before = DateTime.UtcNow;
        var result = _provider.UtcNow;
        var after = DateTime.UtcNow;

        result.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Now_ShouldAlwaysBeAheadOfUtcNow()
    {
        var now = _provider.Now;
        var utcNow = _provider.UtcNow;

        now.Should().BeAfter(utcNow);
    }
}