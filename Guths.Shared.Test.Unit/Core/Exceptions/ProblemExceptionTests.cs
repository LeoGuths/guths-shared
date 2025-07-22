using Guths.Shared.Core.Exceptions;

namespace Guths.Shared.Test.Unit.Core.Exceptions;

public sealed class ProblemExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessageAndError_WhenErrorProvided()
    {
        const string message = "Something went wrong";
        const string error = "CUSTOM_ERROR";

        var exception = new ProblemException(message, error);

        Assert.Equal(message, exception.Message);
        Assert.Equal(error, exception.Error);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_ShouldSetDefaultError_WhenErrorIsNull()
    {
        const string message = "Another error";

        var exception = new ProblemException(message);

        Assert.Equal(message, exception.Message);
        Assert.Equal("UNSPECIFIED_ERROR", exception.Error);
    }

    [Fact]
    public void Constructor_ShouldSetInnerException_WhenProvided()
    {
        const string message = "With inner exception";
        var inner = new InvalidOperationException("inner");

        var exception = new ProblemException(message, null, inner);

        Assert.Equal(message, exception.Message);
        Assert.Equal("UNSPECIFIED_ERROR", exception.Error);
        Assert.Same(inner, exception.InnerException);
    }
}
