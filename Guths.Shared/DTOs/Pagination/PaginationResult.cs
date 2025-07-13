namespace Guths.Shared.DTOs.Pagination;

public sealed record PaginationResult<TResult>
{
    public ICollection<TResult> Items { get; set; } = null!;
    public PaginationData Pagination { get; set; } = null!;
}
