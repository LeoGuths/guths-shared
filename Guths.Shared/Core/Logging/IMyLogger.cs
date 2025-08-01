// ReSharper disable UnusedTypeParameter
namespace Guths.Shared.Core.Logging;

public interface IMyLogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception? exception, string message, params object[] args);
    void LogDebug(string message, params object[] args);
    void LogCritical(Exception? exception, string message, params object[] args);
    IDisposable? BeginScope<TState>(TState state) where TState : notnull;

    void LogInformationWithProperties(string message, object properties);
    void LogErrorWithProperties(Exception? exception, string message, object properties);
}
