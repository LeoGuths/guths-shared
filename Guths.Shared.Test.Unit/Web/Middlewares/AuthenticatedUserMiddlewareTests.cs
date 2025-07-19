using System.Security.Claims;

using Guths.Shared.Authentication.Models;
using Guths.Shared.Core.Constants;
using Guths.Shared.Web.Middlewares;

using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Test.Unit.Web.Middlewares;

public sealed class AuthenticatedUserMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_UserAuthenticated_SetsAuthenticatedUserInHttpContextItems()
    {
        var calledNext = false;

        var claims = new List<Claim>
        {
            new(Config.Claim.IdentityIdClaimType, "user1"),
            new(Config.Claim.TenantConfigIdClaimType, "tenant123"),
            new(Config.Claim.ClientIdClaimType, "clientA"),
            new(Config.Claim.ResourceIdClaimType, "resourceX"),
            new(Config.Claim.UserTokenTypeClaimType, Config.Claim.RefreshToken)
        };

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext
        {
            User = principal
        };

        var middleware = new AuthenticatedUserMiddleware(_ =>
        {
            calledNext = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        Assert.True(calledNext);
        Assert.True(context.Items.ContainsKey(Const.Api.Header.AuthenticatedUserHeaderName));

        var authenticatedUser = context.Items[Const.Api.Header.AuthenticatedUserHeaderName] as AuthenticatedUser;

        Assert.NotNull(authenticatedUser);
        Assert.Equal("user1", authenticatedUser!.UserId);
        Assert.Equal("tenant123", authenticatedUser.TenantConfigId);
        Assert.Equal("clientA", authenticatedUser.ClientId);
        Assert.Equal("resourceX", authenticatedUser.ResourceId);
        Assert.Equal(Config.Claim.RefreshToken, authenticatedUser.UserTokenType);
    }

    [Fact]
    public async Task InvokeAsync_UserNotAuthenticated_DoesNotSetAuthenticatedUser()
    {
        var calledNext = false;

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };

        var middleware = new AuthenticatedUserMiddleware(_ =>
        {
            calledNext = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        Assert.True(calledNext);
        Assert.False(context.Items.ContainsKey(Const.Api.Header.AuthenticatedUserHeaderName));
    }
}
