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
    [InlineData(0, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(20, 10, 1)]
    [InlineData(100, 10, 9)]
    public void LastPage_ShouldReturnCorrectValue(long totalRecords, int pageSize, int expectedLastPage)
    {
        var data = new PaginationData
        {
            TotalRecords = totalRecords,
            PageSize = pageSize
        };

        Assert.Equal(expectedLastPage, data.LastPage);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(5, true)]
    public void HasPreviousPage_ShouldReturnTrue_WhenPageNumberGreaterOrEqual2(int pageNumber, bool expected)
    {
        var data = new PaginationData { PageNumber = pageNumber };
        Assert.Equal(expected, data.HasPreviousPage);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(7, true)]
    [InlineData(8, false)]
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


    [Fact]
    public void PreviousPage_ShouldReturnFirstPage_WhenNoPreviousPage()
    {
        var data = new PaginationData { PageNumber = 1 };
        Assert.Equal(PaginationData.FirstPage, data.PreviousPage);
    }

    [Fact]
    public void PreviousPage_ShouldReturnPageNumberMinusOne_WhenHasPreviousPage()
    {
        var data = new PaginationData { PageNumber = 3 };
        Assert.Equal(2, data.PreviousPage);
    }

    [Fact]
    public void NextPage_ShouldReturnLastPage_WhenNoNextPage()
    {
        var data = new PaginationData { PageNumber = 9, TotalRecords = 90, PageSize = 10 };
        Assert.Equal(data.LastPage, data.NextPage);
    }

    [Fact]
    public void NextPage_ShouldReturnPageNumberPlusOne_WhenHasNextPage()
    {
        var data = new PaginationData { PageNumber = 5, TotalRecords = 90, PageSize = 10 };
        Assert.Equal(6, data.NextPage);
    }
}
