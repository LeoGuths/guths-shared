using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class SharedConfigurationExtensions
{

    /// <summary>
    /// Registers all shared service-level configurations such as authentication,
    /// CORS, caching, localization, controllers, logging, HTTP resilience, and more.
    /// Should be called during the host building phase (IHostApplicationBuilder).
    /// </summary>
    public static void AddSharedConfiguration(this IHostApplicationBuilder builder,
        SharedConfigurationOptions? options = null)
    {
        builder.ConfigureApplication();

        builder.Services.AddHttpContextAccessor();

        builder.UseAuthSetup(options?.AuthorizationActions);
        builder.AddCacheServices();
        builder.AddCorsServices();
        builder.AddGlobalExceptionServices();
        builder.AddLocalizationServices();
        builder.Services.AddJsonServices();
        builder.AddScalarServices();
        builder.AddControllerServices();
        builder.AddDefaultHttpResilience();
        builder.AddLoggingServices();
        builder.AddCoravelServices();
    }

    /// <summary>
    /// Configures the request pipeline using the registered shared services.
    /// Should be called on the WebApplication instance after building.
    /// </summary>
    public static void AddSharedConfiguration(this WebApplication app,
        SharedConfigurationOptions? options = null)
    {
        app.UseRouting();

        app.UseAuthSetup();
        app.AddCorsPolicy();
        app.UseScalar();
        app.UseGlobalExceptionHandling();
        app.UseControllerMappings();
        app.UseLocalizationServices();
        app.UseCoravelScheduler(options?.SchedulerActions);
    }
}
