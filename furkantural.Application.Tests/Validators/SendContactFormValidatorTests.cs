using FluentAssertions;
using FluentValidation.TestHelper;
using furkantural.Application.Features.Contact.Commands;
using furkantural.Application.Features.Contact.Validators;
namespace furkantural.Application.Tests.Validators;

public class SendContactFormValidatorTests
{
    private readonly SendContactFormValidator _validator = new();

    private static SendContactFormCommand CreateValidCommand() => new()
    {
        Email = "test@example.com",
        NameSurname = "Test User",
        MessageNeed = "Hello, I need help.",
        TurnstileResponse = "valid-token",
        IpAddress = "127.0.0.1"
    };

    #region Email

    [Fact]
    public async Task Email_WhenEmpty_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.Email = string.Empty;

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Email_WhenInvalidFormat_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.Email = "not-an-email";

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Email_WhenValid_ShouldNotHaveError()
    {
        var command = CreateValidCommand();

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region NameSurname

    [Fact]
    public async Task NameSurname_WhenEmpty_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.NameSurname = string.Empty;

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.NameSurname);
    }

    [Fact]
    public async Task NameSurname_WhenExceeds100Chars_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.NameSurname = new string('A', 101);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.NameSurname);
    }

    [Fact]
    public async Task NameSurname_WhenExactly100Chars_ShouldNotHaveError()
    {
        var command = CreateValidCommand();
        command.NameSurname = new string('A', 100);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.NameSurname);
    }

    #endregion

    #region MessageNeed

    [Fact]
    public async Task MessageNeed_WhenEmpty_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.MessageNeed = string.Empty;

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MessageNeed);
    }

    [Fact]
    public async Task MessageNeed_WhenExceeds2000Chars_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.MessageNeed = new string('A', 2001);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.MessageNeed);
    }

    [Fact]
    public async Task MessageNeed_WhenExactly2000Chars_ShouldNotHaveError()
    {
        var command = CreateValidCommand();
        command.MessageNeed = new string('A', 2000);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MessageNeed);
    }

    #endregion

    #region TurnstileResponse

    [Fact]
    public async Task TurnstileResponse_WhenEmpty_ShouldHaveError()
    {
        var command = CreateValidCommand();
        command.TurnstileResponse = string.Empty;

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.TurnstileResponse);
    }

    [Fact]
    public async Task TurnstileResponse_WhenProvided_ShouldNotHaveError()
    {
        var command = CreateValidCommand();

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(x => x.TurnstileResponse);
    }

    #endregion

    #region FullValidation

    [Fact]
    public async Task ValidCommand_ShouldPassAllRules()
    {
        var command = CreateValidCommand();

        var result = await _validator.TestValidateAsync(command);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task AllFieldsEmpty_ShouldHaveMultipleErrors()
    {
        var command = new SendContactFormCommand();

        var result = await _validator.TestValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(4);
    }

    #endregion
}