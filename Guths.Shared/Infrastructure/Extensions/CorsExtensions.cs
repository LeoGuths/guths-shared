using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Core.Constants;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class CorsExtensions
{

    /// <summary>
    /// Registers CORS services using the settings defined under SharedConfiguration:CorsConfiguration.
    /// </summary>
    public static void AddCorsServices(this IHostApplicationBuilder builder)
    {
        var sharedConfig = builder.Configuration.GetSection("SharedConfiguration");
        var useCors = sharedConfig.GetValue("UseCors", false);

        if (!useCors)
            return;

        var corsSection = sharedConfig.GetSection("CorsConfiguration");
        var origins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? [];

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(Const.Application.CorsPolicyName, policy =>
            {
                if (origins.Length == 0)
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();

                    return;
                }

                policy
                    .WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }

    /// <summary>
    /// Enables the CORS policy in the request pipeline using the configured policy name.
    /// </summary>
    public static void AddCorsPolicy(this WebApplication app)
    {
        if (!app.Configuration.GetValue("SharedConfiguration:UseCors", false))
            return;

        app.UseCors(Const.Application.CorsPolicyName);
    }
}
