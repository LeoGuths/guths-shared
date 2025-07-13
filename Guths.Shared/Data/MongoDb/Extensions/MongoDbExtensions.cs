using Guths.Shared.DTOs.Pagination;
using Guths.Shared.Helpers;

using MongoDB.Driver;

namespace Guths.Shared.Data.MongoDb.Extensions;

public static class MongoDbExtensions
{
    public static async Task<(ICollection<T> Items, long TotalCount)> GetPagedAsync<T>(
        this IMongoCollection<T> collection,
        PaginationInput paginationInput,
        CancellationToken cancellationToken,
        FilterDefinition<T>? filter = null) where T : class
    {
        filter ??= FilterDefinitionHelper.New<T>();

        var totalCountTask = collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var itemsTask = collection.Find(filter)
            .Skip((paginationInput.PageNumber - 1) * paginationInput.PageSize)
            .Limit(paginationInput.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        await Task.WhenAll(totalCountTask, itemsTask);

        var totalCount = totalCountTask.Result;

        return (totalCount > 0 ? itemsTask.Result : [], totalCount);
    }
}
