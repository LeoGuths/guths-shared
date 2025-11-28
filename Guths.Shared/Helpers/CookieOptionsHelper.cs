using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Helpers;

public static class CookieOptionsHelper
{
    public static CookieOptions GetCookieOptions(double? expirationInMinutes = null)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expirationInMinutes is not null
                ? DateTimeOffset.UtcNow.AddMinutes(expirationInMinutes.Value)
                : DateTimeOffset.UtcNow.AddDays(7)
        };
    }
}
