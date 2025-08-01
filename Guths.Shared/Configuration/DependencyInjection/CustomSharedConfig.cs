using System.Diagnostics.CodeAnalysis;

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
        builder.AddAuthConfiguration(options?.ConfigureAuthorization);
        builder.AddCacheConfiguration();
        builder.AddCorsConfiguration();
        builder.AddGlobalExceptionConfiguration();
        builder.Services.AddProblemDetails();
        builder.AddLocalizationConfiguration();
        builder.Services.AddJsonConfiguration();
        builder.AddScalarConfiguration();
        builder.AddControllerConfiguration();
        builder.AddDefaultResilienceConfiguration();
        builder.AddLoggingConfiguration();
    }

    public static void AddSharedConfiguration(this WebApplication app, string projectName)
    {
        app.UseRouting();
        app.AddAuthConfiguration();
        app.AddCorsConfiguration();
        app.AddScalarConfiguration(projectName);
        app.AddGlobalExceptionConfiguration();
        app.AddControllerConfiguration();
        app.AddLocalizationConfiguration();
    }
}

public sealed class SharedConfigurationOptions
{
    public Action<AuthorizationOptions>? ConfigureAuthorization { get; init; }
}
