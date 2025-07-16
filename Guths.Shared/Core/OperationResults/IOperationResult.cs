namespace Guths.Shared.Core.OperationResults;

public interface IOperationResult
{
    bool IsSuccess { get; }
    List<string> Messages { get; }
    public List<ValidationError> Validations { get; }

    bool HasFailReturn();
}
