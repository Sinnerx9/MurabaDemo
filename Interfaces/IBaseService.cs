using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Interfaces;

public interface IBaseService
{
    public Task<TEntity> InsertAsync<TEntity>(TEntity entity, bool transaction = false,
        CancellationToken ct = default) where TEntity : class,new();

    public Task<TOriginal> UpdateAsync<TID, TOriginal, TModified>(TID id, TModified modified, bool transaction = false,
        CancellationToken ct = default) where TOriginal : class,new() where TModified : class,new() where TID : struct;

    public Task<(IEnumerable<TEntity>, int)> GetAsync<TQuery, TEntity>(TQuery query, PaginationQuery pagination,  CancellationToken ct = default) where TEntity : class,new();

    public Task<TEntity?> ShowAsync<TID, TEntity>(TID id, CancellationToken ct = default) where TEntity : class,new() where TID : struct;

    public Task<TEntity> DeleteAsync<TID, TEntity>(TID id, CancellationToken ct = default) where TEntity : class,new() where TID : struct;
}