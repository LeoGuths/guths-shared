using System.Diagnostics.CodeAnalysis;

namespace Guths.Shared.Core.Constants;

[ExcludeFromCodeCoverage]
public static class Const
{
    public static class Application
    {
        public const string CorsPolicyName = "All";
    }

    public static class Api
    {
        public static class Header
        {
            public const string UserLanguageHeaderName = "X-User-Language";
            public const string UserTimeZoneHeaderName = "X-User-TimeZone";
            public const string AuthenticatedUserHeaderName = "AuthenticatedUser";
        }
    }

    public static class Lang
    {
        public const string PtBr = "pt-BR";
        public const string EnUs = "en-US";
        public const string EsEs = "es-ES";
    }

    public static class File
    {
        public const string ContentType = "Content-Type";
    }

    public static class TimeAndDate
    {
        public const int HourInSeconds = 3600;
        public const int MinuteInSeconds = 60;
        public const double S3PreSignedUrlDuration = 6;

        public const string DefaultTimeZoneId = "America/Sao_Paulo";
    }
}
