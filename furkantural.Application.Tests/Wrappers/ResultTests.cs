using FluentAssertions;
using furkantural.Application.Wrappers;
namespace furkantural.Application.Tests.Wrappers;

public class ResultTests
{
    [Fact]
    public void Ok_ShouldReturnSuccessResult()
    {
        var result = Result.Ok("done");

        result.Success.Should().BeTrue();
        result.Message.Should().Be("done");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Ok_WithoutMessage_ShouldReturnEmptyMessage()
    {
        var result = Result.Ok();

        result.Success.Should().BeTrue();
        result.Message.Should().BeEmpty();
    }

    [Fact]
    public void Fail_WithSingleError_ShouldReturnFailResult()
    {
        var result = Result.Fail("something went wrong");

        result.Success.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Be("something went wrong");
    }

    [Fact]
    public void Fail_WithMultipleErrors_ShouldReturnAllErrors()
    {
        var errors = new List<string> { "error1", "error2", "error3" };
        var result = Result.Fail(errors);

        result.Success.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void GenericOk_ShouldReturnDataAndSuccess()
    {
        var result = Result<int>.Ok(42, "found");

        result.Success.Should().BeTrue();
        result.Data.Should().Be(42);
        result.Message.Should().Be("found");
    }

    [Fact]
    public void GenericFail_ShouldReturnNullDataAndError()
    {
        var result = Result<string>.Fail("not found");

        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Errors.Should().ContainSingle().Which.Should().Be("not found");
    }

    [Fact]
    public void GenericFail_WithMultipleErrors_ShouldReturnAllErrors()
    {
        var errors = new List<string> { "err1", "err2" };
        var result = Result<string>.Fail(errors);

        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Errors.Should().HaveCount(2);
    }
}