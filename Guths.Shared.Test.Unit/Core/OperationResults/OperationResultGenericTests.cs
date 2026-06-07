using FluentValidation.Results;
using Guths.Shared.Core.OperationResults;

namespace Guths.Shared.Test.Unit.Core.OperationResults;

public sealed class OperationResultGenericTests
{
    private class Dummy
    {
        public string Name { get; set; } = "Test";
    }

    [Fact]
    public void Success_WithValue_ShouldSetIsSuccessAndValue()
    {
        var dummy = new Dummy();
        var result = OperationResult<Dummy>.Success(dummy);

        Assert.True(result.IsSuccess);
        Assert.Equal(dummy, result.Value);
        Assert.False(result.IsForbidden);
        Assert.False(result.IsNotFound);
    }

    [Fact]
    public void Fail_WithMessage_ShouldSetMessageAndIsSuccessFalse()
    {
        var result = OperationResult<Dummy>.Fail("error");

        Assert.False(result.IsSuccess);
        Assert.Contains("error", result.Messages);
        Assert.Null(result.Value);
        Assert.True(result.HasFailReturn());
    }

    [Fact]
    public void Fail_WithValidation_ShouldSetValidation()
    {
        var validation = new ValidationFailure("Age", "Must be positive");
        var result = OperationResult<Dummy>.Fail([validation]);

        Assert.Single(result.Validations);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Forbid_ShouldSetIsForbiddenTrue()
    {
        var result = OperationResult<Dummy>.Forbid();

        Assert.False(result.IsSuccess);
        Assert.True(result.IsForbidden);
        Assert.False(result.IsNotFound);
    }

    [Fact]
    public void NotFound_ShouldSetIsNotFoundTrue()
    {
        var result = OperationResult<Dummy>.NotFound("Not found");

        Assert.False(result.IsSuccess);
        Assert.False(result.IsForbidden);
        Assert.True(result.IsNotFound);
        Assert.Contains("Not found", result.Messages);
    }
}
