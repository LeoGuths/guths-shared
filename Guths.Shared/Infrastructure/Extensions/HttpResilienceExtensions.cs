using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Retry;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class HttpResilienceExtensions
{

    /// <summary>
    /// Registers the default HTTP resilience policy using a retry strategy,
    /// exponential backoff with jitter, and a global timeout.
    ///
    /// This extension should be called during the Build phase
    /// (IHostApplicationBuilder) to configure resilience pipelines
    /// before any HttpClient or service is resolved.
    /// </summary>
    public static void AddDefaultHttpResilience(this IHostApplicationBuilder builder)
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
