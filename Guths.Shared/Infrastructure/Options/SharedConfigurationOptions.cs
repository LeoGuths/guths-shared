using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Guths.Shared.Infrastructure.Options;

/// <summary>
/// Represents optional configuration hooks used when applying the shared setup.
/// Allows injection of custom authorization rules and scheduler configuration.
/// </summary>
public sealed class SharedConfigurationOptions
{
    /// <summary>
    /// Optional callback to configure Authorization policies (roles, requirements, etc).
    /// </summary>
    public Action<AuthorizationOptions>? AuthorizationActions { get; init; }

    /// <summary>
    /// Optional callback to configure Coravel's task scheduler.
    /// </summary>
    public Action<IScheduler>? SchedulerActions { get; init; } = null!;
}
