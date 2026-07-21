using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Helpers;

public static class CookieOptionsHelper
{
    public static CookieOptions GetCookieOptions(DateTimeOffset expires, bool isLocal = false)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isLocal,
            SameSite = isLocal ? SameSiteMode.Lax : SameSiteMode.None,
            Path = "/",
            Expires = expires
        };
    }
}
