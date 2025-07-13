using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Guths.Shared.Core.Constants;
using Guths.Shared.Web.Middlewares;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class LocalizationConfig
{
    private static readonly CultureInfo[] _supportedCultures =
    [
        new(Const.Lang.EnUs),
        new(Const.Lang.PtBr),
        new(Const.Lang.EsEs)
    ];

    public static void AddLocalizationConfiguration(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = string.Empty);

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(Const.Lang.EnUs);
            options.SupportedCultures = _supportedCultures;
            options.SupportedUICultures = _supportedCultures;

            options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
            {
                var userLanguage = context.Request.Headers[Const.Api.Header.UserLanguageHeaderName].ToString();
                if (string.IsNullOrWhiteSpace(userLanguage))
                    return Task.FromResult<ProviderCultureResult>(null!)!;

                var userLanguages = userLanguage.Split(',')
                    .Select(lang => lang.Split(';').First().Trim())
                    .ToList();

                var userPreferredCulture = _supportedCultures.FirstOrDefault(culture => userLanguages.Contains(culture.Name));
                return (userPreferredCulture != null
                    ? Task.FromResult(new ProviderCultureResult(userPreferredCulture.Name, userPreferredCulture.Name))
                    : Task.FromResult<ProviderCultureResult>(null!))!;
            }));
        });
    }

    public static void AddLocalizationSupport(this WebApplication app)
    {
        var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
        if (localizationOptions != null)
            app.UseRequestLocalization(localizationOptions);

        app.UseMiddleware<LocalizationMiddleware>(_supportedCultures.ToList());
    }
}
