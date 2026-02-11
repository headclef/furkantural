using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using furkantural.Application.Common.Behaviors;
using furkantural.Application.Wrappers;
using MediatR;
using Moq;
namespace furkantural.Application.Tests.Behaviors;

public class TestRequest : IRequest<Result> { public string Name { get; set; } = string.Empty; }

public class ValidationBehaviorTests
{

    #region NoValidators

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNext()
    {
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, Result>(validators);
        var expectedResult = Result.Ok("success");

        var result = await behavior.Handle(
            new TestRequest(),
            () => Task.FromResult(expectedResult),
            CancellationToken.None);

        result.Should().BeSameAs(expectedResult);
    }

    #endregion

    #region ValidationPasses

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNext()
    {
        var validator = new Mock<IValidator<TestRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, Result>(new[] { validator.Object });
        var expectedResult = Result.Ok("passed");

        var result = await behavior.Handle(
            new TestRequest { Name = "Valid" },
            () => Task.FromResult(expectedResult),
            CancellationToken.None);

        result.Should().BeSameAs(expectedResult);
    }

    #endregion

    #region ValidationFails

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailResult()
    {
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Email", "Email is invalid")
        };

        var validator = new Mock<IValidator<TestRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var behavior = new ValidationBehavior<TestRequest, Result>(new[] { validator.Object });
        var nextCalled = false;

        var result = await behavior.Handle(
            new TestRequest(),
            () => { nextCalled = true; return Task.FromResult(Result.Ok()); },
            CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain("Name is required");
        result.Errors.Should().Contain("Email is invalid");
        nextCalled.Should().BeFalse("next should not be called when validation fails");
    }

    #endregion

    #region MultipleValidators

    [Fact]
    public async Task Handle_WhenMultipleValidatorsFail_ShouldAggregateErrors()
    {
        var validator1 = new Mock<IValidator<TestRequest>>();
        validator1
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("A", "Error A") }));

        var validator2 = new Mock<IValidator<TestRequest>>();
        validator2
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("B", "Error B") }));

        var behavior = new ValidationBehavior<TestRequest, Result>(new[] { validator1.Object, validator2.Object });

        var result = await behavior.Handle(
            new TestRequest(),
            () => Task.FromResult(Result.Ok()),
            CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain("Error A");
        result.Errors.Should().Contain("Error B");
    }

    #endregion
}