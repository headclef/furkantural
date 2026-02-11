using FluentAssertions;
using furkantural.Application.Repositories;
using furkantural.Application.Services.Abstract;
using furkantural.Domain.Entities;
using furkantural.Infrastructure.Services.Concrete;
using Microsoft.AspNetCore.Http;
using Moq;
namespace furkantural.Infrastructure.Tests.Services;

public class LogServiceTests
{
    private readonly Mock<IRepository<Log>> _logRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IDateTimeProvider> _dateTime = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly LogService _logService;

    public LogServiceTests()
    {
        _dateTime.Setup(d => d.Now).Returns(new DateTime(2026, 1, 1, 12, 0, 0));
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>())).Returns(Task.CompletedTask);

        _logService = new LogService(
            _logRepository.Object,
            _unitOfWork.Object,
            _dateTime.Object,
            _httpContextAccessor.Object);
    }

    [Fact]
    public async Task Info_ShouldLogWithInfoLevel()
    {
        Log? captured = null;
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .Callback<Log>(l => captured = l)
            .Returns(Task.CompletedTask);

        await _logService.Info("test message", "detail");

        captured.Should().NotBeNull();
        captured!.Level.Should().Be("Info");
        captured.Message.Should().Be("test message");
        captured.Detail.Should().Be("detail");
    }

    [Fact]
    public async Task Error_ShouldLogWithErrorLevel()
    {
        Log? captured = null;
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .Callback<Log>(l => captured = l)
            .Returns(Task.CompletedTask);

        await _logService.Error("error occurred");

        captured!.Level.Should().Be("Error");
    }

    [Fact]
    public async Task Success_ShouldLogWithSuccessLevel()
    {
        Log? captured = null;
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .Callback<Log>(l => captured = l)
            .Returns(Task.CompletedTask);

        await _logService.Success("done");

        captured!.Level.Should().Be("Success");
    }

    [Fact]
    public async Task Warning_ShouldLogWithWarningLevel()
    {
        Log? captured = null;
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .Callback<Log>(l => captured = l)
            .Returns(Task.CompletedTask);

        await _logService.Warning("careful");

        captured!.Level.Should().Be("Warning");
    }

    [Fact]
    public async Task LogAsync_ShouldCallSaveChanges()
    {
        await _logService.Info("test");

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_ShouldSetProjectName()
    {
        Log? captured = null;
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .Callback<Log>(l => captured = l)
            .Returns(Task.CompletedTask);

        await _logService.Info("test");

        captured!.Project.Should().Be("FurkanTural");
    }

    [Fact]
    public async Task LogAsync_WhenRepositoryThrows_ShouldNotBubbleException()
    {
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .ThrowsAsync(new Exception("DB down"));

        var action = async () => await _logService.Info("test");

        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task LogAsync_ShouldCaptureIpFromHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.1");
        _httpContextAccessor.Setup(h => h.HttpContext).Returns(httpContext);

        Log? captured = null;
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .Callback<Log>(l => captured = l)
            .Returns(Task.CompletedTask);

        await _logService.Info("test");

        captured!.IpAddress.Should().Be("10.0.0.1");
    }

    [Fact]
    public async Task LogAsync_ShouldCapturePathFromHttpContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/Base/Index";
        _httpContextAccessor.Setup(h => h.HttpContext).Returns(httpContext);

        Log? captured = null;
        _logRepository.Setup(r => r.AddAsync(It.IsAny<Log>()))
            .Callback<Log>(l => captured = l)
            .Returns(Task.CompletedTask);

        await _logService.Info("test");

        captured!.Path.Should().Be("/Base/Index");
    }
}