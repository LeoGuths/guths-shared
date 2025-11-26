using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Core.Extensions;
using Guths.Shared.Infrastructure.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class RedisCacheExtensions
{
    public static void AddCacheServices(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseCache", false))
            return;

        builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection(nameof(CacheOptions)));

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetRequired("RedisConnection:Host");
            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
            {
                User = builder.Configuration.GetRequired("RedisConnection:User"),
                Password = builder.Configuration.GetRequired("RedisConnection:Password"),
                AbortOnConnectFail = true,
                EndPoints = { options.Configuration! }
            };
        });
    }
}
