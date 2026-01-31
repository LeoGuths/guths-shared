using Guths.Shared.Authentication.Models;
using Guths.Shared.Core.Constants;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Guths.Shared.Web.Middlewares;

public sealed class AuthenticatedUserMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticatedUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var loggerFactory = context.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
        var logger = loggerFactory?.CreateLogger<AuthenticatedUserMiddleware>();

        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
        logger?.LogInformation("AuthenticatedUserMiddleware: IsAuthenticated={IsAuthenticated}", isAuthenticated);

        if (isAuthenticated)
        {
            var claimsCount = context.User.Claims.Count();
            logger?.LogInformation("AuthenticatedUserMiddleware: claimsCount={Count}. Claims: {Claims}",
                claimsCount,
                string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}")));

            context.Items[Const.Api.Header.AuthenticatedUserHeaderName] = new AuthenticatedUser(context.User.Claims.ToArray());
        }

        await _next(context);
    }
}
