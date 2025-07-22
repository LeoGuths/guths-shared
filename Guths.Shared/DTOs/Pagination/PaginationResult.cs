using System.Diagnostics.CodeAnalysis;

namespace Guths.Shared.DTOs.Pagination;

[ExcludeFromCodeCoverage]
public sealed record PaginationResult<TResult>
{
    public ICollection<TResult> Items { get; set; } = null!;
    public PaginationData Pagination { get; set; } = null!;
}
