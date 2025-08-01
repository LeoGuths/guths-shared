using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Polly;
using Polly.Retry;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class HttpConfig
{
    public static void AddDefaultResilienceConfiguration(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseDefaultHttpResilience", false))
            return;

        builder.Services.AddResiliencePipeline("default", x =>
        {
            x.AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                    Delay = TimeSpan.FromMilliseconds(200),
                    MaxRetryAttempts = 2,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                })
                .AddTimeout(TimeSpan.FromSeconds(30));
        });
    }
}
