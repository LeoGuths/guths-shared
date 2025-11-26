using System.Diagnostics.CodeAnalysis;
using Guths.Shared.Authentication.OpenApi;
using Guths.Shared.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Guths.Shared.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class ScalarExtensions
{
    private const string UseScalarPath = "SharedConfiguration:UseScalar";

    /// <summary>
    /// Registers Scalar/OpenAPI services, including optional JWT security scheme integration.
    /// </summary>
    public static void AddScalarServices(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue(UseScalarPath, false))
            return;

        builder.Services.AddOpenApi(opts =>
        {
            if (builder.Configuration.GetValue("SharedConfiguration:UseAuth", false))
                opts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
    }

    /// <summary>
    /// Maps Scalar UI and OpenAPI endpoints into the application's request pipeline.
    /// </summary>
    public static void UseScalar(this WebApplication app)
    {
        if (!app.Configuration.GetValue(UseScalarPath, false))
            return;

        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle(AssemblyExtensions.GetProjectName())
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
