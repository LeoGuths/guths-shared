using Guths.Shared.Core.OperationResults;

namespace Guths.Shared.Test.Unit.Core.OperationResults;

public sealed class OperationResultTests
{
    [Fact]
    public void Success_ShouldSetIsSuccessTrue()
    {
        var result = OperationResult.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsForbidden);
        Assert.Empty(result.Messages);
        Assert.Empty(result.Validations);
    }

    [Fact]
    public void Fail_WithMessage_ShouldSetIsSuccessFalse_AndMessage()
    {
        var result = OperationResult.Fail("Something went wrong");

        Assert.False(result.IsSuccess);
        Assert.Contains("Something went wrong", result.Messages);
        Assert.True(result.HasFailReturn());
    }

    [Fact]
    public void Fail_WithValidation_ShouldSetIsSuccessFalse_AndValidation()
    {
        var validation = new ValidationError { Field = "Email", Message = "Invalid format" };
        var result = OperationResult.Fail(validation);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Validations);
        Assert.True(result.HasFailReturn());
    }

    [Fact]
    public void Forbid_ShouldSetIsForbidden()
    {
        var result = OperationResult.Forbid();

        Assert.False(result.IsSuccess);
        Assert.True(result.IsForbidden);
    }
}
