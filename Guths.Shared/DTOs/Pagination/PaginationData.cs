namespace Guths.Shared.DTOs.Pagination;

public sealed record PaginationData
{
    public long TotalRecords { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public int TotalPages
        => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
