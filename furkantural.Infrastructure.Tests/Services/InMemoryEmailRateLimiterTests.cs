using FluentAssertions;
using furkantural.Application.Models;
using furkantural.Application.Services.Abstract;
using furkantural.Infrastructure.Services.Concrete;
using Microsoft.Extensions.Options;
using Moq;
namespace furkantural.Infrastructure.Tests.Services;

public class InMemoryEmailRateLimiterTests
{
    private readonly Mock<IDateTimeProvider> _dateTime = new();

    private InMemoryEmailRateLimiter CreateLimiter(int perHourLimit)
    {
        var options = Options.Create(new SmtpOptions { PerHourLimit = perHourLimit });
        return new InMemoryEmailRateLimiter(options, _dateTime.Object);
    }

    [Fact]
    public async Task CanSendExternalEmail_WhenUnderLimit_ShouldReturnTrue()
    {
        _dateTime.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);
        var limiter = CreateLimiter(5);

        var result = await limiter.CanSendExternalEmail();

        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanSendExternalEmail_WhenAtLimit_ShouldReturnFalse()
    {
        _dateTime.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);
        var limiter = CreateLimiter(2);

        await limiter.CanSendExternalEmail(); // 1
        await limiter.CanSendExternalEmail(); // 2

        var result = await limiter.CanSendExternalEmail(); // 3 → over limit

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanSendExternalEmail_AfterWindowExpires_ShouldAllowAgain()
    {
        var now = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _dateTime.Setup(d => d.UtcNow).Returns(now);

        var limiter = CreateLimiter(1);

        await limiter.CanSendExternalEmail(); // Uses the slot
        var blocked = await limiter.CanSendExternalEmail(); // Should be blocked
        blocked.Should().BeFalse();

        // Advance time by 1 hour + 1 second
        _dateTime.Setup(d => d.UtcNow).Returns(now.AddHours(1).AddSeconds(1));

        var result = await limiter.CanSendExternalEmail();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanSendExternalEmail_ShouldDequeueOldEntries()
    {
        var now = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _dateTime.Setup(d => d.UtcNow).Returns(now);

        var limiter = CreateLimiter(2);

        await limiter.CanSendExternalEmail(); // t=12:00

        _dateTime.Setup(d => d.UtcNow).Returns(now.AddMinutes(30));
        await limiter.CanSendExternalEmail(); // t=12:30

        // At t=13:01, the first entry (12:00) should be expired
        _dateTime.Setup(d => d.UtcNow).Returns(now.AddHours(1).AddMinutes(1));

        var result = await limiter.CanSendExternalEmail(); // Should pass (only 12:30 entry remains)
        result.Should().BeTrue();
    }
}