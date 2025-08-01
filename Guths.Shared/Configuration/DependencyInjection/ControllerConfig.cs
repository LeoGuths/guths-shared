using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Web.Attributes;
using Guths.Shared.Web.Formatters;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ControllerConfig
{
    public static void AddControllerConfiguration(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseControllers", false))
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

    public static void AddControllerConfiguration(this WebApplication app)
    {
        if (!app.Configuration.GetValue("SharedConfiguration:UseControllers", false))
            return;

        app.MapControllers();
    }
}
