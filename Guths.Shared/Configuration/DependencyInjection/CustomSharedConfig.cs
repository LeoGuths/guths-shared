using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Authentication.OpenApi;
using Guths.Shared.Core.Constants;
using Guths.Shared.Web.Handlers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class CustomSharedConfig
{
    public static void AddSharedConfiguration(this IServiceCollection services, ConfigurationManager configuration,
        SharedConfigurationOptions? options = null)
    {
        options ??= new SharedConfigurationOptions();

        services.AddHttpContextAccessor();

        if (options.UseAuth)
            services.AddAuthConfiguration(configuration, options.ConfigureAuthorization);

        if (options.UseCache)
            services.AddCacheConfiguration(configuration);

        if (options.UseCors)
            services.AddCorsConfiguration();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        if (options.UseLocalization)
            services.AddLocalizationConfiguration();

        services.AddJsonConfiguration();

        if (options.UseScalar)
            services.AddOpenApi(opts =>
            {
                if (options.UseAuth)
                    opts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

        if (options.UseControllers)
            services.AddControllerConfiguration();

        services.AddDefaultResilienceConfiguration();
    }

    public static void AddSharedConfiguration(this WebApplication app,
        string projectName, SharedConfigurationOptions? options = null)
    {
        options ??= new SharedConfigurationOptions();

        app.UseRouting();

        if(options.UseAuth)
            app.AddAuthConfiguration();

        if(options.UseCors)
            app.UseCors(Const.Application.CorsPolicyName);

        if (options.UseScalar)
            app.AddScalarConfiguration(projectName);

        app.UseExceptionHandler();

        if (options.UseControllers)
            app.MapControllers();

        if (options.UseLocalization)
            app.AddLocalizationSupport();
    }
}

public sealed class SharedConfigurationOptions
{
    public bool UseCache { get; init; } = true;
    public bool UseAuth { get; init; } = true;
    public bool UseCors { get; init; } = true;
    public bool UseControllers { get; init; } = true;
    public bool UseLocalization { get; init; } = true;
    public bool UseScalar { get; init; } = true;

    public Action<AuthorizationOptions>? ConfigureAuthorization { get; init; }
}
