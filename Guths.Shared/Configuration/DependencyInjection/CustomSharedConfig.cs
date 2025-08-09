using System.Diagnostics.CodeAnalysis;

using Coravel.Scheduling.Schedule.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class CustomSharedConfig
{
    public static void AddSharedConfiguration(this IHostApplicationBuilder builder,
        SharedConfigurationOptions? options = null)
    {
        builder.ConfigureApplication();
        builder.Services.AddHttpContextAccessor();
        builder.AddAuthConfiguration(options?.AuthorizationActions);
        builder.AddCacheConfiguration();
        builder.AddCorsConfiguration();
        builder.AddGlobalExceptionConfiguration();
        builder.AddLocalizationConfiguration();
        builder.Services.AddJsonConfiguration();
        builder.AddScalarConfiguration();
        builder.AddControllerConfiguration();
        builder.AddDefaultResilienceConfiguration();
        builder.AddLoggingConfiguration();
        builder.AddCoravelConfiguration();
    }

    public static void AddSharedConfiguration(this WebApplication app,
        SharedConfigurationOptions? options = null)
    {
        app.UseRouting();
        app.AddAuthConfiguration();
        app.AddCorsConfiguration();
        app.AddScalarConfiguration();
        app.AddGlobalExceptionConfiguration();
        app.AddControllerConfiguration();
        app.AddLocalizationConfiguration();
        app.AddCoravelConfiguration(options?.SchedulerActions);
    }
}

public sealed class SharedConfigurationOptions
{
    public Action<AuthorizationOptions>? AuthorizationActions { get; init; }
    public Action<IScheduler>? SchedulerActions { get; init; } = null!;
}
