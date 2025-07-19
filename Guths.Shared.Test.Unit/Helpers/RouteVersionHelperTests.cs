using Guths.Shared.Helpers;

namespace Guths.Shared.Test.Unit.Helpers;

public sealed class RouteVersionHelperTests
{
    [Theory]
    [InlineData("v1", "users", "v1/users")]
    [InlineData("v1", "/users", "v1/users")]
    [InlineData("v1", "//users", "v1/users")]
    [InlineData("v1", "", "v1/")]
    [InlineData("", "users", "/users")]
    public void BuildRoute_ShouldBuildExpectedRoute(string version, string route, string expected)
    {
        var result = RouteVersionHelper.BuildRoute(version, route);

        Assert.Equal(expected, result);
    }
}
