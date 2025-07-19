using Guths.Shared.Core.OperationResults;

namespace Guths.Shared.Test.Unit.Core.OperationResults;

public sealed class ValidationErrorTests
{
    [Fact]
    public void ValidationError_ShouldStoreFieldAndMessage()
    {
        var error = new ValidationError
        {
            Field = "Name",
            Message = "Name is required"
        };

        Assert.Equal("Name", error.Field);
        Assert.Equal("Name is required", error.Message);
    }
}
