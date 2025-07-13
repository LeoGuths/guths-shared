using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Guths.Shared.Core.Constants;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Guths.Shared.Web.Middlewares;

[ExcludeFromCodeCoverage]
public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICollection<CultureInfo> _supportedCultures;

    public LocalizationMiddleware(RequestDelegate next, ICollection<CultureInfo> supportedCultures)
    {
        _next = next;
        _supportedCultures = supportedCultures;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userLanguage = context.Request.Headers[Const.Api.Header.UserLanguageHeaderName].ToString();
        if (!string.IsNullOrWhiteSpace(userLanguage))
        {
            var userLanguages = userLanguage.Split(',')
                .Select(lang => lang.Split(';').First().Trim())
                .ToList();

            var userPreferredCulture = _supportedCultures.FirstOrDefault(culture => userLanguages.Contains(culture.Name));

            if (userPreferredCulture != null)
            {
                var requestCulture = new RequestCulture(userPreferredCulture);
                var requestCultureFeature = new RequestCultureFeature(requestCulture, null);
                context.Features.Set<IRequestCultureFeature>(requestCultureFeature);
            }
        }
        await _next(context);
    }
}
