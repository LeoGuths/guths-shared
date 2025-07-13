using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Retry;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class HttpConfig
{
    public static void AddDefaultResilienceConfiguration(this IServiceCollection services)
    {
        services.AddResiliencePipeline("default", x =>
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
