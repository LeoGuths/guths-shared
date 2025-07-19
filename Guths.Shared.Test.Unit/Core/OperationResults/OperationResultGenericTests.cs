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
        var validation = new ValidationError { Field = "Age", Message = "Must be positive" };
        var result = OperationResult<Dummy>.Fail(validation);

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
    }
}
