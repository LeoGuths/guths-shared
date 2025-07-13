namespace Guths.Shared.DTOs.Pagination;

public sealed record PaginationData
{
    public long TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages
        => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);

    public static int FirstPage => 1;
    public int LastPage => TotalPages is 0 or 1 ? 1 : TotalPages - 1;
    public bool HasPreviousPage => PageNumber >= 2;
    public bool HasNextPage => PageNumber < LastPage;

    public int PreviousPage => !HasPreviousPage ? FirstPage : PageNumber - 1;

    public int NextPage => !HasNextPage ? LastPage : PageNumber + 1;
}
