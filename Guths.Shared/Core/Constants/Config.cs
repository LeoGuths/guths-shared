namespace Guths.Shared.Core.Constants;

public static class Config
{
    public static class AppConfigKey
    {
        public const string MongoConnectionString = "DatabaseConnection:MongoDbConnectionString";
        public const string MongoDb = "DatabaseConnection:MongoDbDatabaseName";

        public const string CloudApplicationConfig = "AmazonSystemsStore";
        public const string LocalApplicationConfig = "appsettings.Local.json";
    }

    public static class Claim
    {
        public const string IdentityIdClaimType = "identity_id";
        public const string TenantConfigIdClaimType = "tenant_config_id";
        public const string ClientIdClaimType = "client_id";
        public const string ResourceIdClaimType = "resource_id";
        public const string UserTokenTypeClaimType = "typ";
        public const string JtiClaimType = "jti";

        public const string RefreshToken = "refresh";
        public const string User = "user";
        public const string System = "system";
    }

    public static class Scope
    {
        public const string SendEmail = "send_email";
    }
}
