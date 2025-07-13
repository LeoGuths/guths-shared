using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Builder;

using Scalar.AspNetCore;

namespace Guths.Shared.Configuration.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ScalarConfig
{
    // https://github.com/scalar/scalar/blob/main/documentation/integrations/dotnet.md
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
