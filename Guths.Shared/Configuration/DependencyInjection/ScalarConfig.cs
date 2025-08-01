using System.Diagnostics.CodeAnalysis;

using Guths.Shared.Authentication.OpenApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Scalar.AspNetCore;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ScalarConfig
{
    public static void AddScalarConfiguration(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseScalar", false))
            return;

        builder.Services.AddOpenApi(opts =>
        {
            if (builder.Configuration.GetValue("SharedConfiguration:UseAuth", false))
                opts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
    }

    public static void AddScalarConfiguration(this WebApplication app, string projectName)
    {
        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle(projectName)
                .WithSidebar()
                .WithTheme(ScalarTheme.Moon)
                .WithDarkMode()
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .WithDarkModeToggle(false)
                .AddPreferredSecuritySchemes("Bearer");
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/")
            {
                context.Response.Redirect("/scalar/v1", permanent: false);
                return;
            }
            await next();
        });
    }
}
