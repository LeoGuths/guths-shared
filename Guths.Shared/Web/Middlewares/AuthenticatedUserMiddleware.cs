using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Authentication.Models;
using Guths.Shared.Core.Constants;

using Microsoft.AspNetCore.Http;

namespace Guths.Shared.Web.Middlewares;

[ExcludeFromCodeCoverage]
public class AuthenticatedUserMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticatedUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
            context.Items[Const.Api.Header.AuthenticatedUserHeaderName] = new AuthenticatedUser(context.User.Claims.ToArray());

        await _next(context);
    }
}
