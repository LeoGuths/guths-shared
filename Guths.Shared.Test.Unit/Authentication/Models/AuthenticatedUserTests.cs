using System.Security.Claims;

using Guths.Shared.Authentication.Models;
using Guths.Shared.Core.Constants;

namespace Guths.Shared.Test.Unit.Authentication.Models;

public sealed class AuthenticatedUserTests
{
    [Fact]
    public void Constructor_ShouldPopulateProperties_WhenClaimsAreValid()
    {
        var claims = new[]
        {
            new Claim(Config.Claim.IdentityIdClaimType, "user1"),
            new Claim(Config.Claim.TenantConfigIdClaimType, "tenant1"),
            new Claim(Config.Claim.ClientIdClaimType, "client1"),
            new Claim(Config.Claim.ResourceIdClaimType, "resource1"),
            new Claim(Config.Claim.UserTokenTypeClaimType, Config.Claim.RefreshToken)
        };

        var user = new AuthenticatedUser(claims);

        Assert.Equal("user1", user.UserId);
        Assert.Equal("tenant1", user.TenantConfigId);
        Assert.Equal("client1", user.ClientId);
        Assert.Equal("resource1", user.ResourceId);
        Assert.Equal(Config.Claim.RefreshToken, user.UserTokenType);
    }

    [Fact]
    public void IsInvalid_ShouldReturnFalse_WhenRequiredClaimsExist()
    {
        var claims = ValidClaims();
        var user = new AuthenticatedUser(claims);

        Assert.False(user.IsInvalid());
    }

    [Theory]
    [InlineData(Config.Claim.IdentityIdClaimType)]
    [InlineData(Config.Claim.TenantConfigIdClaimType)]
    [InlineData(Config.Claim.ClientIdClaimType)]
    public void IsInvalid_ShouldReturnTrue_WhenRequiredClaimIsMissing(string missingClaimType)
    {
        var claims = ValidClaims().Where(c => c.Type != missingClaimType).ToList();
        var user = new AuthenticatedUser(claims);

        Assert.True(user.IsInvalid());
    }

    [Fact]
    public void IsInvalidForRefresh_ShouldReturnFalse_WhenValidRefreshToken()
    {
        var claims = ValidClaims();
        var user = new AuthenticatedUser(claims);

        Assert.False(user.IsInvalidForRefresh());
    }

    [Fact]
    public void IsInvalidForRefresh_ShouldReturnTrue_WhenTokenTypeIsNotRefresh()
    {
        var claims = ValidClaims()
            .Where(c => c.Type != Config.Claim.UserTokenTypeClaimType)
            .Append(new Claim(Config.Claim.UserTokenTypeClaimType, "access_token"))
            .ToList();

        var user = new AuthenticatedUser(claims);

        Assert.True(user.IsInvalidForRefresh());
    }

    private static List<Claim> ValidClaims() =>
    [
        new(Config.Claim.IdentityIdClaimType, "user1"),
        new(Config.Claim.TenantConfigIdClaimType, "tenant1"),
        new(Config.Claim.ClientIdClaimType, "client1"),
        new(Config.Claim.ResourceIdClaimType, "resource1"),
        new(Config.Claim.UserTokenTypeClaimType, Config.Claim.RefreshToken)
    ];
}
