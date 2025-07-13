using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Web.Attributes;
using Guths.Shared.Web.Formatters;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ControllerConfig
{
    public static void AddControllerConfiguration(this IServiceCollection services)
    {
        services.AddControllers(
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
}
