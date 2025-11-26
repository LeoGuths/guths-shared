using System.Diagnostics.CodeAnalysis;
using Guths.Shared.Web.Attributes;
using Guths.Shared.Web.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ControllersExtensions
{
    private const string UseControllersPath = "SharedConfiguration:UseControllers";

    /// <summary>
    /// Registers MVC controllers, input/output formatters, filters, and JSON settings.
    /// </summary>
    public static void AddControllerServices(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue(UseControllersPath, false))
            return;

        builder.Services.AddControllers(
            options => {
                options.InputFormatters.Insert(0, new TextMediaInputFormatter());
                options.OutputFormatters.Insert(0, new TextMediaOutputFormatter());
                options.Filters.Add<TrimModelAttribute>();
            }).AddNewtonsoftJson(options => {
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.UseMemberCasing();
        });
    }

    /// <summary>
    /// Maps controller endpoints into the application's request pipeline.
    /// </summary>
    public static void UseControllerMappings(this WebApplication app)
    {
        if (!app.Configuration.GetValue(UseControllersPath, false))
            return;

        app.MapControllers();
    }
}
