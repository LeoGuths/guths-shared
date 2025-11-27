using System.Diagnostics.CodeAnalysis;

namespace Guths.Shared.Core.Constants;

[ExcludeFromCodeCoverage]
public static class Config
{
    public static class AppConfigKey
    {
        public const string MongoConnectionString = "DatabaseConnection:MongoDbConnectionString";
        public const string MongoDb = "DatabaseConnection:MongoDbDatabaseName";
    }

    public static class Claim
    {
        public const string IdentityIdClaimType = "identity_id";
        public const string TenantConfigIdClaimType = "tenant_config_id";
        public const string ClientIdClaimType = "client_id";
        public const string ResourceIdClaimType = "resource_id";
        public const string UserTokenTypeClaimType = "typ";

        public const string RefreshToken = "refresh";
    }

    public static class Auth
    {
        public const string AccessTokenCookieName = "access_token";
        public const string RefreshTokenCookieName = "refresh_token";
    }
}
