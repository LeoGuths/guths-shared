namespace Guths.Shared.Core.Exceptions;

public sealed class ProblemException : Exception
{
    public string Error { get; }
    public override string Message { get; }

    public ProblemException(string message, string? error = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Error = error ?? "UNSPECIFIED_ERROR";
        Message = message;
    }
}
