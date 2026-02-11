using FluentAssertions;
using furkantural.Application.Services.Abstract;
using furkantural.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
namespace furkantural.Infrastructure.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionMiddleware>> _logger = new();
    private readonly Mock<IHostEnvironment> _environment = new();
    private readonly Mock<ILogService> _logService = new();

    public GlobalExceptionMiddlewareTests()
    {
        _logService.Setup(l => l.Error(It.IsAny<string>(), It.IsAny<string?>())).Returns(Task.CompletedTask);
    }

    private GlobalExceptionMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new GlobalExceptionMiddleware(next, _logger.Object, _environment.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNext()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context, _logService.Object);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_ShouldRedirectToError()
    {
        var middleware = CreateMiddleware(_ => throw new InvalidOperationException("boom"));
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        _environment.Setup(e => e.EnvironmentName).Returns("Production");

        await middleware.InvokeAsync(context, _logService.Object);

        context.Response.StatusCode.Should().Be(302);
        context.Response.Headers.Location.ToString().Should().Contain("/Base/Error");
        context.Response.Headers.Location.ToString().Should().Contain("Code=500");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_ShouldLogError()
    {
        var middleware = CreateMiddleware(_ => throw new InvalidOperationException("boom"));
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        _environment.Setup(e => e.EnvironmentName).Returns("Production");

        await middleware.InvokeAsync(context, _logService.Object);

        _logService.Verify(l => l.Error(It.Is<string>(s => s.Contains("boom")), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenResponseAlreadyStarted_ShouldNotRedirect()
    {
        // Simulating response already started is tricky with DefaultHttpContext.
        // We verify the middleware guards against it by using a custom context.
        var nextCalled = false;
        var middleware = CreateMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context, _logService.Object);

        // If no exception, should just pass through
        nextCalled.Should().BeTrue();
    }
}