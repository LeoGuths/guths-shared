using Guths.Shared.Web.Handlers;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Configuration.DependencyInjection;

public static class GlobalExceptionConfig
{
    public static void AddGlobalExceptionConfiguration(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseGlobalExceptionHandler", false))
            return;

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    }

    public static void AddGlobalExceptionConfiguration(this WebApplication app)
    {
        if (!app.Configuration.GetValue("SharedConfiguration:UseGlobalExceptionHandler", false))
            return;

        app.UseExceptionHandler();
    }
}
