using System.Linq.Expressions;

using Guths.Shared.DTOs.Pagination;

using MongoDB.Driver;

namespace Guths.Shared.Core.Domain.Interfaces;

public interface IRepository<T>
{
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(string id, Expression<Func<T, T>>? projection = null, CancellationToken cancellationToken = default);
    Task<TResult?> GetByIdAsync<TResult>(string id, Expression<Func<T, TResult>>? projection = null, CancellationToken cancellationToken = default);
    Task<ICollection<T>> GetAllAsync(FilterDefinition<T>? filterDefinition = null, CancellationToken cancellationToken = default);
    Task<(ICollection<T> Items, long TotalCount)> GetPagedAsync(PaginationInput paginationInput, FilterDefinition<T>? filterDefinition = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Task>> CursorByProcessAsync(int batchSize, Func<List<T>, Task> action, Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
}
