using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Helpers;

public static class CookieOptionsHelper
{
    public static CookieOptions GetCookieOptions(DateTimeOffset expires)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = expires
        };
    }
}
