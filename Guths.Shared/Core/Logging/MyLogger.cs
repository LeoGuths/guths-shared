using Microsoft.Extensions.Logging;

namespace Guths.Shared.Core.Logging;

public sealed class MyLogger<T> : IMyLogger<T>
{
    private readonly ILogger<T> _logger;

    public MyLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args) =>
        _logger.LogInformation(message, args);

    public void LogWarning(string message, params object[] args) =>
        _logger.LogWarning(message, args);

    public void LogError(Exception? exception, string message, params object[] args) =>
        _logger.LogError(exception, message, args);

    public void LogDebug(string message, params object[] args) =>
        _logger.LogDebug(message, args);

    public void LogCritical(Exception? exception, string message, params object[] args) =>
        _logger.LogCritical(exception, message, args);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
        _logger.BeginScope(state);

    public void LogInformationWithProperties(string message, object properties) =>
        _logger.LogInformation(message + " {@Properties}", properties);

    public void LogErrorWithProperties(Exception? exception, string message, object properties) =>
        _logger.LogError(exception, message + " {@Properties}", properties);
}
