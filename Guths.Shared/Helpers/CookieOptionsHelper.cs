using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Helpers;

public static class CookieOptionsHelper
{
    public static CookieOptions GetCookieOptions(double? expirationInHours = null)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expirationInHours is not null
                ? DateTimeOffset.UtcNow.AddHours(expirationInHours.Value)
                : DateTimeOffset.UtcNow.AddDays(7)
        };
    }
}
