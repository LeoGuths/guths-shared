using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.Extensions.DependencyInjection;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class JsonConfig
{
    public static void AddJsonConfiguration(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}
