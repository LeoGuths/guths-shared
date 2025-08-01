using System.ComponentModel;

namespace Guths.Shared.Core.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> items, int page, int size)
        => items.Skip((page - 1) * size).Take(size);

    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) =>
        source switch
        {
            null => true,
            ICollection<T> c => c.Count == 0,
            IReadOnlyCollection<T> r => r.Count == 0,
            _ => !source.Any()
        };

    public static IEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, ListSortDirection direction = ListSortDirection.Descending)
        => direction == ListSortDirection.Ascending ? source.OrderBy(selector) : source.OrderByDescending(selector);
}
