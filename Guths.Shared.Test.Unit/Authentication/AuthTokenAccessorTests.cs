using Moq;
using Microsoft.AspNetCore.Http;

using Guths.Shared.Authentication;

namespace Guths.Shared.Test.Unit.Authentication;

public sealed class AuthTokenAccessorTests
{

    [Fact]
    public void AccessToken_ShouldReturnToken_WhenHeaderIsValid()
    {
        const string expectedToken = "abc.def.ghi";
        var headers = new HeaderDictionary
        {
            ["Authorization"] = $"Bearer {expectedToken}"
        };

        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(r => r.Headers).Returns(headers);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.SetupGet(c => c.Request).Returns(requestMock.Object);

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);

        var tokenAccessor = new AuthTokenAccessor(accessorMock.Object);

        var token = tokenAccessor.AccessToken;

        Assert.Equal(expectedToken, token);
    }

    [Fact]
    public void AccessToken_ShouldThrowException_WhenHeaderIsMissing()
    {
        var headers = new HeaderDictionary();

        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(r => r.Headers).Returns(headers);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.SetupGet(c => c.Request).Returns(requestMock.Object);

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);

        var tokenAccessor = new AuthTokenAccessor(accessorMock.Object);

        var exception = Assert.Throws<Exception>(() => _ = tokenAccessor.AccessToken);
        Assert.Equal("Invalid Authorization Header", exception.Message);
    }

    [Fact]
    public void AccessToken_ShouldThrowException_WhenHeaderDoesNotStartWithBearer()
    {
        var headers = new HeaderDictionary
        {
            ["Authorization"] = "Basic xyz123"
        };

        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(r => r.Headers).Returns(headers);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.SetupGet(c => c.Request).Returns(requestMock.Object);

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);

        var tokenAccessor = new AuthTokenAccessor(accessorMock.Object);

        var exception = Assert.Throws<Exception>(() => _ = tokenAccessor.AccessToken);
        Assert.Equal("Invalid Authorization Header", exception.Message);
    }

    [Fact]
    public void AccessToken_ShouldReturnCachedValue_OnSubsequentCalls()
    {
        const string expectedToken = "abc.def.ghi";
        var headers = new HeaderDictionary
        {
            ["Authorization"] = $"Bearer {expectedToken}"
        };

        var requestMock = new Mock<HttpRequest>();
        requestMock.SetupGet(r => r.Headers).Returns(headers);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.SetupGet(c => c.Request).Returns(requestMock.Object);

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.SetupGet(a => a.HttpContext).Returns(httpContextMock.Object);

        var tokenAccessor = new AuthTokenAccessor(accessorMock.Object);

        var firstCall = tokenAccessor.AccessToken;
        var secondCall = tokenAccessor.AccessToken;

        Assert.Equal(expectedToken, firstCall);
        Assert.Equal(expectedToken, secondCall);
        requestMock.Verify(r => r.Headers, Times.Once);
    }
}
