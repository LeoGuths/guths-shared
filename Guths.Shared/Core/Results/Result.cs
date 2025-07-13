namespace Guths.Shared.Core.Results;

public sealed class Result<T> : IResult where T : class
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public bool IsForbidden { get; }
    public List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    private Result(bool isSuccess, T? value, bool isForbidden, List<string>? messages = null, List<ValidationError>? validations = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        IsForbidden = isForbidden;
        Messages = messages ?? [];
        Validations = validations ?? [];
    }

    public static Result<T> Success(T? value) => new(true, value, false);
    public static Result<T> Fail(List<string> messages) => new(false, null, false, messages: messages);
    public static Result<T> Fail(string? message = null) => new(false, null, false, !string.IsNullOrWhiteSpace(message) ? [message] : []);
    public static Result<T> Fail(List<ValidationError> validations) => new(false, null, false, validations: validations);
    public static Result<T> Fail(ValidationError validation) => new(false, null, false, validations: [validation]);
    public static Result<T> Forbid() => new(false, null, true);

    public bool HasFailReturn() =>
        Validations is { Count: > 0 } || Messages is { Count: > 0 };
}

public class Result : IResult
{
    public bool IsSuccess { get; }
    public bool IsForbidden { get; }
    public List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    private Result(bool isSuccess, bool isForbidden, List<string>? messages = null, List<ValidationError>? validations = null)
    {
        IsSuccess = isSuccess;
        IsForbidden = isForbidden;
        Messages = messages ?? [];
        Validations = validations ?? [];
    }

    public static Result Success() => new(true,false,[]);
    public static Result Fail(List<string> messages) => new(false,false, messages);
    public static Result Fail(string? message = null) => new(false,false, !string.IsNullOrWhiteSpace(message) ? [message] : []);
    public static Result Fail(List<ValidationError> validations) => new(false,false, validations: validations);
    public static Result Fail(ValidationError validation) => new(false,false, validations: [validation]);
    public static Result Forbid() => new(false, true);

    public bool HasFailReturn() =>
        Validations is { Count: > 0 } || Messages is { Count: > 0 };
}
