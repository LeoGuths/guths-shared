namespace Guths.Shared.Core.OperationResults;

public sealed class OperationResult<T> : IOperationResult where T : class
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsForbidden { get; }
    public List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    private OperationResult(bool isSuccess, T? value, bool isForbidden, List<string>? messages = null, List<ValidationError>? validations = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        IsForbidden = isForbidden;
        Messages = messages ?? [];
        Validations = validations ?? [];
    }

    public static OperationResult<T> Success(T? value) => new(true, value, false);
    public static OperationResult<T> Fail(List<string> messages) => new(false, null, false, messages: messages);
    public static OperationResult<T> Fail(string? message = null) => new(false, null, false, !string.IsNullOrWhiteSpace(message) ? [message] : []);
    public static OperationResult<T> Fail(List<ValidationError> validations) => new(false, null, false, validations: validations);
    public static OperationResult<T> Fail(ValidationError validation) => new(false, null, false, validations: [validation]);
    public static OperationResult<T> Forbid() => new(false, null, true);

    public bool HasFailReturn() =>
        Validations is { Count: > 0 } || Messages is { Count: > 0 };
}

public class OperationResult : IOperationResult
{
    public bool IsSuccess { get; }
    public bool IsForbidden { get; }
    public List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    private OperationResult(bool isSuccess, bool isForbidden, List<string>? messages = null, List<ValidationError>? validations = null)
    {
        IsSuccess = isSuccess;
        IsForbidden = isForbidden;
        Messages = messages ?? [];
        Validations = validations ?? [];
    }

    public static OperationResult Success() => new(true,false,[]);
    public static OperationResult Fail(List<string> messages) => new(false,false, messages);
    public static OperationResult Fail(string? message = null) => new(false,false, !string.IsNullOrWhiteSpace(message) ? [message] : []);
    public static OperationResult Fail(List<ValidationError> validations) => new(false,false, validations: validations);
    public static OperationResult Fail(ValidationError validation) => new(false,false, validations: [validation]);
    public static OperationResult Forbid() => new(false, true);

    public bool HasFailReturn() =>
        Validations is { Count: > 0 } || Messages is { Count: > 0 };
}

