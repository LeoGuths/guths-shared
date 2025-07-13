namespace Guths.Shared.Core.Results;

public sealed class ValidationError
{
    public string Field { get; set; } = null!;
    public string Message { get; set; } = null!;
}
