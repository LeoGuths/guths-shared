using System.Linq.Expressions;

using Guths.Shared.Core.Domain.Entities;
using Guths.Shared.Core.Domain.Interfaces;
using Guths.Shared.Data.MongoDb.Extensions;
using Guths.Shared.DTOs.Pagination;

using MongoDB.Driver;

namespace Guths.Shared.Data.MongoDb;

public class MongoDbRepository<T> : IRepository<T> where T : class
{
    protected readonly IMongoCollection<T> Collection;


    protected MongoDbRepository(IMongoDatabase database, string collectionName) =>
        Collection = database.GetCollection<T>(collectionName);

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity is BaseEntity document)
            document.Id = Ulid.NewUlid().ToString();

        await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        return entity;
    }

    public async Task<T?> GetByIdAsync(string id, Expression<Func<T, T>>? projection = null, CancellationToken cancellationToken = default) =>
        projection is null
            ? await Collection.Find(IdFilter(id)).FirstOrDefaultAsync(cancellationToken: cancellationToken)
            : await Collection.Find(IdFilter(id)).Project(projection).FirstOrDefaultAsync(cancellationToken: cancellationToken);

    public async Task<TResult?> GetByIdAsync<TResult>(string id, Expression<Func<T, TResult>>? projection = null, CancellationToken cancellationToken = default)
    {
        if (projection is not null)
            return await Collection
                .Find(IdFilter(id))
                .Project(projection)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        var entity = await Collection
            .Find(IdFilter(id))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return entity is null ? default : (TResult)(object)entity;
    }

    public async Task<ICollection<T>> GetAllAsync(FilterDefinition<T>? filterDefinition = null, CancellationToken cancellationToken = default) =>
        await Collection.Find(filterDefinition ?? Builders<T>.Filter.Empty).ToListAsync(cancellationToken);

    public async Task<(ICollection<T> Items, long TotalCount)> GetPagedAsync(PaginationInput paginationInput,
        FilterDefinition<T>? filter = null, CancellationToken cancellationToken = default) =>
        await Collection.GetPagedAsync(paginationInput, cancellationToken, filter);

    public async Task<IEnumerable<Task>> CursorByProcessAsync(int batchSize, Func<List<T>, Task> action,
        Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();

        var cursor = await Collection
            .Find(filter ?? (_ => true), new FindOptions { BatchSize = batchSize })
            .ToCursorAsync(cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            var values = cursor.Current.ToList();
            tasks.Add(action(values));
        }

        return tasks.ToArray();
    }

    public async Task<bool> UpdateAsync(string id, T entity, CancellationToken cancellationToken = default)
    {
        var result = await Collection.ReplaceOneAsync(IdFilter(id), entity, cancellationToken: cancellationToken);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await Collection.DeleteOneAsync(IdFilter(id), cancellationToken);

        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<bool> ExistsByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await Collection.CountDocumentsAsync(IdFilter(id), cancellationToken: cancellationToken) > 0;

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) =>
        await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0;

    public FilterDefinition<T> IdFilter(string id) =>
        Builders<T>.Filter.Eq("_id", id);
}
