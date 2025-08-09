using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Configuration.DependencyInjection;

public static class CoravelConfig
{
    public static void AddCoravelConfiguration(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue("SharedConfiguration:UseCoravel", false))
            return;

        builder.Services.AddScheduler();
        builder.Services.AddQueue();
    }

    public static void AddCoravelConfiguration(this WebApplication app, Action<IScheduler>? scheduler)
    {
        if (!app.Configuration.GetValue("SharedConfiguration:UseCoravel", false))
            return;

        if (scheduler is null)
            throw new InvalidOperationException("Missing required scheduler configuration");

        app.Services.UseScheduler(scheduler);
    }
}
