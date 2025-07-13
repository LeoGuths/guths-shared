namespace Guths.Shared.DTOs.Pagination;

public sealed record PaginationInput()
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PaginationInput(int pageNumber, int pageSize) : this()
    {
        PageSize = pageSize;
        PageNumber = pageNumber;
    }

    public override int GetHashCode() =>
        HashCode.Combine(PageNumber, PageSize);
}
