using Guths.Shared.DTOs.Pagination;

namespace Guths.Shared.Test.Unit.DTOs.Pagination;

public sealed class PaginationDataTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(100, 10, 10)]
    [InlineData(101, 10, 11)]
    [InlineData(99, 0, 0)]
    public void TotalPages_ShouldCalculateCorrectly(long totalRecords, int pageSize, int expected)
    {
        var data = new PaginationData
        {
            TotalRecords = totalRecords,
            PageSize = pageSize
        };

        Assert.Equal(expected, data.TotalPages);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(5, true)]
    public void HasPreviousPage_ShouldReturnTrue_WhenPageNumberGreaterOrEqual2(int pageNumber, bool expected)
    {
        var data = new PaginationData { PageNumber = pageNumber, TotalRecords = 100, PageSize = 10 };
        Assert.Equal(expected, data.HasPreviousPage);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(7, true)]
    [InlineData(8, true)]
    [InlineData(9, false)]
    public void HasNextPage_ShouldReturnTrue_WhenPageNumberLessThanLastPage(int pageNumber, bool expected)
    {
        var data = new PaginationData
        {
            PageNumber = pageNumber,
            TotalRecords = 90,
            PageSize = 10
        };
        Assert.Equal(expected, data.HasNextPage);
    }
}
