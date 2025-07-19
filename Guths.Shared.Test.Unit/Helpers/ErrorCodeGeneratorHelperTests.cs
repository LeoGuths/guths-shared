using Guths.Shared.Helpers;

namespace Guths.Shared.Test.Unit.Helpers;

public sealed class ErrorCodeGeneratorHelperTests
{
    [Fact]
    public void GenerateErrorCode_ShouldReturnNonEmptyString()
    {
        var code = ErrorCodeGeneratorHelper.GenerateErrorCode();

        Assert.False(string.IsNullOrWhiteSpace(code));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(64)]
    public void GenerateErrorCode_ShouldReturnStringWithCorrectLength(int length)
    {
        var code = ErrorCodeGeneratorHelper.GenerateErrorCode(length);

        Assert.Equal(length, code.Length);
    }

    [Fact]
    public void GenerateErrorCode_ShouldContainOnlyValidCharacters()
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var code = ErrorCodeGeneratorHelper.GenerateErrorCode(100);

        Assert.All(code, c => Assert.Contains(c, validChars));
    }

    [Fact]
    public void GenerateErrorCode_ShouldGenerateDifferentValues()
    {
        var code1 = ErrorCodeGeneratorHelper.GenerateErrorCode();
        var code2 = ErrorCodeGeneratorHelper.GenerateErrorCode();

        Assert.NotEqual(code1, code2);
    }
}
