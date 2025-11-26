using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Guths.Shared.Infrastructure.Extensions;

public static class CoravelExtensions
{
    private const string UseCoravelPath = "SharedConfiguration:UseCoravel";

    /// <summary>
    /// Registers Coravel components such as the Scheduler and Queue services.
    /// </summary>
    public static void AddCoravelServices(this IHostApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue(UseCoravelPath, false))
            return;

        builder.Services.AddScheduler();
        builder.Services.AddQueue();
    }

    /// <summary>
    /// Activates the Coravel scheduler and applies the provided scheduling configuration.
    /// </summary>
    /// <param name="app">The running WebApplication instance.</param>
    /// <param name="scheduler">
    /// Delegate used to configure scheduled tasks.
    /// Must not be null when Coravel is enabled.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when Coravel is enabled but no scheduler configuration delegate is supplied.
    /// </exception>
    public static void UseCoravelScheduler(this WebApplication app, Action<IScheduler>? scheduler)
    {
        if (!app.Configuration.GetValue(UseCoravelPath, false))
            return;

        if (scheduler is null)
            throw new InvalidOperationException("Missing required scheduler configuration");

        app.Services.UseScheduler(scheduler);
    }
}
