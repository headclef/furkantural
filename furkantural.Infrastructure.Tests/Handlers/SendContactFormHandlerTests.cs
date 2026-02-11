using FluentAssertions;
using furkantural.Application.Features.Contact.Commands;
using furkantural.Application.Models;
using furkantural.Application.Services.Abstract;
using furkantural.Application.Wrappers;
using furkantural.Infrastructure.Features.Contact.Handlers;
using Microsoft.Extensions.Options;
using Moq;
namespace furkantural.Infrastructure.Tests.Handlers;

public class SendContactFormHandlerTests
{
    private readonly Mock<IOptions<SmtpOptions>> _smtpOptions = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<ITurnstileService> _turnstileService = new();
    private readonly Mock<ILogService> _logService = new();
    private readonly Mock<IDateTimeProvider> _dateTime = new();
    private readonly SendContactFormHandler _handler;

    public SendContactFormHandlerTests()
    {
        _smtpOptions.Setup(o => o.Value).Returns(new SmtpOptions
        {
            ListenerEmail = "admin@test.com"
        });

        _dateTime.Setup(d => d.Now).Returns(new DateTime(2026, 1, 15, 14, 30, 0));

        _logService.Setup(l => l.Info(It.IsAny<string>(), It.IsAny<string?>())).Returns(Task.CompletedTask);
        _logService.Setup(l => l.Error(It.IsAny<string>(), It.IsAny<string?>())).Returns(Task.CompletedTask);
        _logService.Setup(l => l.Success(It.IsAny<string>(), It.IsAny<string?>())).Returns(Task.CompletedTask);

        _handler = new SendContactFormHandler(
            _smtpOptions.Object,
            _emailService.Object,
            _turnstileService.Object,
            _logService.Object,
            _dateTime.Object);
    }

    private static SendContactFormCommand CreateValidCommand() => new()
    {
        Email = "user@example.com",
        NameSurname = "Test User",
        MessageNeed = "Hello",
        TurnstileResponse = "token-123",
        IpAddress = "192.168.1.1"
    };

    #region TurnstileValidation

    [Fact]
    public async Task Handle_WhenTurnstileFails_ShouldReturnFail()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("Bot detected"));

        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Bot detected");
    }

    [Fact]
    public async Task Handle_WhenTurnstileFails_ShouldNotSendEmails()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("Bot detected"));

        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        _emailService.Verify(
            e => e.SendTransactionalEmailAsync(It.IsAny<string>(), It.IsAny<Domain.Enums.EmailType>(), It.IsAny<Dictionary<string, string>>()),
            Times.Never);
    }

    #endregion

    #region SuccessfulSubmission

    [Fact]
    public async Task Handle_WhenEverythingSucceeds_ShouldReturnOk()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync(It.IsAny<string>(), It.IsAny<Domain.Enums.EmailType>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Ok());

        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Message.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldSendBothEmails()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync(It.IsAny<string>(), It.IsAny<Domain.Enums.EmailType>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Ok());

        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        _emailService.Verify(
            e => e.SendTransactionalEmailAsync("admin@test.com", Domain.Enums.EmailType.Listener, It.IsAny<Dictionary<string, string>>()),
            Times.Once);
        _emailService.Verify(
            e => e.SendTransactionalEmailAsync("user@example.com", Domain.Enums.EmailType.Contact, It.IsAny<Dictionary<string, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldLogSuccess()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync(It.IsAny<string>(), It.IsAny<Domain.Enums.EmailType>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Ok());

        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        _logService.Verify(l => l.Success(It.Is<string>(s => s.Contains("user@example.com")), It.IsAny<string?>()), Times.Once);
    }

    #endregion

    #region EmailFailures

    [Fact]
    public async Task Handle_WhenBothEmailsFail_ShouldReturnFail()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync(It.IsAny<string>(), It.IsAny<Domain.Enums.EmailType>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Fail("SMTP error"));

        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenBothEmailsFail_ShouldLogError()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync(It.IsAny<string>(), It.IsAny<Domain.Enums.EmailType>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Fail("SMTP error"));

        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        _logService.Verify(l => l.Error(It.Is<string>(s => s.Contains("failed")), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOnlyUserEmailSucceeds_ShouldReturnOk()
    {
        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        // Admin email fails, user email succeeds
        _emailService
            .Setup(e => e.SendTransactionalEmailAsync("admin@test.com", Domain.Enums.EmailType.Listener, It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Fail("SMTP error"));
        _emailService
            .Setup(e => e.SendTransactionalEmailAsync("user@example.com", Domain.Enums.EmailType.Contact, It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Ok());

        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Only fails when BOTH fail
        result.Success.Should().BeTrue();
    }

    #endregion

    #region Placeholders

    [Fact]
    public async Task Handle_ShouldBuildPlaceholdersWithCorrectValues()
    {
        Dictionary<string, string>? capturedPlaceholders = null;

        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync("admin@test.com", Domain.Enums.EmailType.Listener, It.IsAny<Dictionary<string, string>>()))
            .Callback<string, Domain.Enums.EmailType, Dictionary<string, string>>((_, _, p) => capturedPlaceholders = p)
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync("user@example.com", Domain.Enums.EmailType.Contact, It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Ok());

        var command = CreateValidCommand();
        await _handler.Handle(command, CancellationToken.None);

        capturedPlaceholders.Should().NotBeNull();
        capturedPlaceholders!["NameSurname"].Should().Be("Test User");
        capturedPlaceholders["Email"].Should().Be("user@example.com");
        capturedPlaceholders["MessageNeed"].Should().Be("Hello");
        capturedPlaceholders["IpAddress"].Should().Be("192.168.1.1");
        capturedPlaceholders["SendTime"].Should().Be("2026.01.15 14:30");
    }

    [Fact]
    public async Task Handle_WhenIpAddressNull_ShouldUseUnknown()
    {
        Dictionary<string, string>? capturedPlaceholders = null;

        _turnstileService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync("admin@test.com", Domain.Enums.EmailType.Listener, It.IsAny<Dictionary<string, string>>()))
            .Callback<string, Domain.Enums.EmailType, Dictionary<string, string>>((_, _, p) => capturedPlaceholders = p)
            .ReturnsAsync(Result.Ok());

        _emailService
            .Setup(e => e.SendTransactionalEmailAsync(It.IsAny<string>(), Domain.Enums.EmailType.Contact, It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Ok());

        var command = CreateValidCommand();
        command.IpAddress = null;
        await _handler.Handle(command, CancellationToken.None);

        capturedPlaceholders!["IpAddress"].Should().Be("Unknown");
    }
    
    #endregion
}