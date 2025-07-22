using Guths.Shared.Core.Exceptions;
using Guths.Shared.Web.Handlers;

namespace Guths.Shared.Test.Unit.Core.Exceptions;

public sealed class ExceptionExtensionsTests
{
    [Fact]
    public void AddErrorCode_ShouldAddNewErrorCode_WhenNotPresent()
    {
        var ex = new Exception();

        ex.AddErrorCode();

        Assert.True(ex.Data.Contains("ErrorCode"));
        Assert.IsType<string>(ex.Data["ErrorCode"]);
        Assert.False(string.IsNullOrWhiteSpace((string)ex.Data["ErrorCode"]!));
    }

    [Fact]
    public void AddErrorCode_ShouldOverrideExistingCode()
    {
        var ex = new Exception
        {
            Data =
            {
                ["ErrorCode"] = "original"
            }
        };

        ex.AddErrorCode();

        var result = (string)ex.Data["ErrorCode"]!;
        Assert.NotEqual("original", result);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GetErrorCode_ShouldReturnCode_WhenPresent()
    {
        var ex = new Exception
        {
            Data =
            {
                ["ErrorCode"] = "CODE123"
            }
        };

        var result = ex.GetErrorCode();

        Assert.Equal("CODE123", result);
    }

    [Fact]
    public void GetErrorCode_ShouldReturnDefault_WhenCodeIsMissing()
    {
        var ex = new Exception();

        var result = ex.GetErrorCode();

        Assert.Equal(GlobalExceptionHandler.UnhandledExceptionMsg, result);
    }
}
