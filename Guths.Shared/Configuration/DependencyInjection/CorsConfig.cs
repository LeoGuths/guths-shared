using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Core.Constants;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class CorsConfig
{
    public static void AddCorsConfiguration(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseCors", false))
            return;

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(Const.Application.CorsPolicyName, policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public static void AddCorsConfiguration(this WebApplication app)
    {
        if (!app.Configuration.GetValue("SharedConfiguration:UseCors", false))
            return;

        app.UseCors(Const.Application.CorsPolicyName);
    }
}
