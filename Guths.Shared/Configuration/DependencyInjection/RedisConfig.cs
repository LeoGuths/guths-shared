using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Configuration.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class RedisConfig
{
    public static void AddCacheConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<CacheOptions>(configuration.GetSection(nameof(CacheOptions)));

        if (configuration.GetValue<bool>("CacheOptions:UseCache"))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisConnection:Host"];
                options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    User = configuration["RedisConnection:User"],
                    Password = configuration["RedisConnection:Password"],
                    AbortOnConnectFail = true,
                    EndPoints = { options.Configuration! }
                };
            });
        }
    }
}
