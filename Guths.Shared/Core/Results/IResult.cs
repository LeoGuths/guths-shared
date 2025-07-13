namespace Guths.Shared.Core.Results;

public interface IResult
{
    bool IsSuccess { get; }
    List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    bool HasFailReturn();
}
