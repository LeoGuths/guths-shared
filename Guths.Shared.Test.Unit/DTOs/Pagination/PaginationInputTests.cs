using Guths.Shared.DTOs.Pagination;

namespace Guths.Shared.Test.Unit.DTOs.Pagination;

public sealed class PaginationInputTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var input = new PaginationInput(2, 25);

        Assert.Equal(2, input.PageNumber);
        Assert.Equal(25, input.PageSize);
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValueForSameProperties()
    {
        var input1 = new PaginationInput(1, 10);
        var input2 = new PaginationInput(1, 10);

        Assert.Equal(input1.GetHashCode(), input2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentValueForDifferentProperties()
    {
        var input1 = new PaginationInput(1, 10);
        var input2 = new PaginationInput(2, 10);

        Assert.NotEqual(input1.GetHashCode(), input2.GetHashCode());
    }
}
