using FluentValidation.Results;

namespace Guths.Shared.Core.OperationResults;

public sealed class OperationResult<T> : IOperationResult where T : class
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsForbidden { get; }
    public bool IsNotFound { get; }
    public List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    private OperationResult(bool isSuccess, T? value, bool isForbidden, bool isNotFound, List<string>? messages = null, List<ValidationError>? validations = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        IsForbidden = isForbidden;
        IsNotFound = isNotFound;
        Messages = messages ?? [];
        Validations = validations ?? [];
    }

    public static OperationResult<T> Success(T? value) => new(true, value, false, false);
    public static OperationResult<T> Fail(string? message = null) => new(false, null, false, false, !string.IsNullOrWhiteSpace(message) ? [message] : []);
    public static OperationResult<T> Fail(IList<ValidationFailure> failures) =>
        new(false, null, false, false, validations: failures
            .Select(f => new ValidationError { Field = f.PropertyName, Message = f.ErrorMessage })
            .ToList());
    public static OperationResult<T> Forbid() => new(false, null, true, false);
    public static OperationResult<T> NotFound(string? message = null) => new(false, null, false, true, !string.IsNullOrWhiteSpace(message) ? [message] : []);

    public bool HasFailReturn() =>
        Validations is { Count: > 0 } || Messages is { Count: > 0 };
}

public class OperationResult : IOperationResult
{
    public bool IsSuccess { get; }
    public bool IsForbidden { get; }
    public bool IsNotFound { get; }
    public List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    private OperationResult(bool isSuccess, bool isForbidden, bool isNotFound, List<string>? messages = null, List<ValidationError>? validations = null)
    {
        IsSuccess = isSuccess;
        IsForbidden = isForbidden;
        IsNotFound = isNotFound;
        Messages = messages ?? [];
        Validations = validations ?? [];
    }

    public static OperationResult Success() => new(true, false, false, []);
    public static OperationResult Fail(List<string> messages) => new(false, false, false, messages);
    public static OperationResult Fail(string? message = null) => new(false, false, false, !string.IsNullOrWhiteSpace(message) ? [message] : []);
    public static OperationResult Fail(IList<ValidationFailure> failures) =>
        new(false, false, false, validations: failures
            .Select(f => new ValidationError { Field = f.PropertyName, Message = f.ErrorMessage })
            .ToList());
    public static OperationResult Forbid() => new(false, true, false);
    public static OperationResult NotFound(string? message = null) => new(false, false, true, !string.IsNullOrWhiteSpace(message) ? [message] : []);

    public bool HasFailReturn() =>
        Validations is { Count: > 0 } || Messages is { Count: > 0 };
}
