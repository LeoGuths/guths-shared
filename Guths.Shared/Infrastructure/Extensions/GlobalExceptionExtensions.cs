using Guths.Shared.Web.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Infrastructure.Extensions;

public static class GlobalExceptionExtensions
{
    private const string UseGlobalExceptionHandlerPath = "SharedConfiguration:UseGlobalExceptionHandler";

    /// <summary>
    /// Registers global exception handling services, including ProblemDetails
    /// customization and the application's custom exception handler.
    /// </summary>
    public static void AddGlobalExceptionServices(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue(UseGlobalExceptionHandlerPath, false))
            return;

        builder.Services.AddProblemDetails(configure =>
        {
            configure.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    }

    /// <summary>
    /// Enables global exception handling middleware in the API pipeline.
    /// </summary>
    public static void UseGlobalExceptionHandling(this WebApplication app)
    {
        if (!app.Configuration.GetValue(UseGlobalExceptionHandlerPath, false))
            return;

        app.UseExceptionHandler();
    }
}
