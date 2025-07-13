using System.Security.Claims;

using Guths.Shared.Core.Constants;

namespace Guths.Shared.Authentication.Models;

public sealed class AuthenticatedUser
{
    public string UserId { get; }
    public string TenantConfigId { get; }
    public string ClientId { get; }
    public string ResourceId { get; }
    public string UserTokenType { get; }

    public AuthenticatedUser(IReadOnlyCollection<Claim> claims)
    {
        UserId = GetClaimValue(claims, Config.Claim.IdentityIdClaimType);
        TenantConfigId = GetClaimValue(claims, Config.Claim.TenantConfigIdClaimType);
        ClientId = GetClaimValue(claims, Config.Claim.ClientIdClaimType);
        ResourceId = GetClaimValue(claims, Config.Claim.ResourceIdClaimType);
        UserTokenType = GetClaimValue(claims, Config.Claim.UserTokenTypeClaimType);
    }

    public bool IsInvalid()
    {
        return string.IsNullOrWhiteSpace(UserId)
               || string.IsNullOrWhiteSpace(ClientId)
               || string.IsNullOrWhiteSpace(TenantConfigId);
    }

    public bool IsInvalidForRefresh()
    {
        return string.IsNullOrWhiteSpace(UserId)
               || string.IsNullOrWhiteSpace(ClientId)
               || string.IsNullOrWhiteSpace(TenantConfigId)
               || UserTokenType != Config.Claim.RefreshToken;
    }

    private static string GetClaimValue(IReadOnlyCollection<Claim> claims, string claimType) =>
        claims.FirstOrDefault(x => x.Type == claimType)?.Value ?? string.Empty;
}
