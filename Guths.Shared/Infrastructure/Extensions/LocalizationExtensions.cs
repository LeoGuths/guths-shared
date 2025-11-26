using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Guths.Shared.Core.Constants;
using Guths.Shared.Web.Middlewares;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class LocalizationExtensions
{
    private static readonly CultureInfo[] _supportedCultures =
    [
        new(Const.Lang.EnUs),
        new(Const.Lang.PtBr),
        new(Const.Lang.EsEs)
    ];

    private const string UseLocalizationPath = "SharedConfiguration:UseLocalization";

    /// <summary>
    /// Registers localization services, supported cultures, default culture,
    /// and the custom request culture provider.
    /// </summary>
    public static void AddLocalizationServices(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue(UseLocalizationPath, false))
            return;

        builder.Services.AddLocalization(options => options.ResourcesPath = string.Empty);

        builder.Services.Configure<RequestLocalizationOptions>(options =>
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

    /// <summary>
    /// Enables localization middleware and applies configured RequestLocalizationOptions.
    /// </summary>
    public static void UseLocalizationServices(this WebApplication app)
    {
        if (!app.Configuration.GetValue(UseLocalizationPath, false))
            return;

        var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
        if (localizationOptions is not null)
            app.UseRequestLocalization(localizationOptions);

        app.UseMiddleware<LocalizationMiddleware>(_supportedCultures.ToList());
    }
}
