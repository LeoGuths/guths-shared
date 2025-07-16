namespace Guths.Shared.Core.OperationResults;

public sealed class ValidationError
{
    public string Field { get; set; } = null!;
    public string Message { get; set; } = null!;
}
