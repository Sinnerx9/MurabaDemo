using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using MurabaDemo.Database.Main;
using MurabaDemo.Database.Tables.Infrastructure;
using MurabaDemo.Helpers.Extensions;
using MurabaDemo.Interfaces;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Services;

public class BaseService(MainDbContext _context) : IBaseService
{


    public async Task<TEntity> InsertAsync<TEntity>(TEntity entity, bool transaction = false, CancellationToken ct = default)
        where TEntity : class, new()
    
    {
        IDbContextTransaction? dbTransaction = null;
        try
        {
            if (transaction)
                dbTransaction = await _context.Database.BeginTransactionAsync(ct);
            var table = _context.Set<TEntity>();
            var entry = await table.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
            if (transaction && dbTransaction != null)
                await dbTransaction.CommitAsync(ct);
            return entry.Entity;
        }
        catch (Exception ex)
        {
            if (transaction && dbTransaction != null)
            {
                await dbTransaction.RollbackAsync(ct);
            }

            throw new Exception("An error occurred while inserting the entity.");
        }
    }

    public async Task<TOriginal> UpdateAsync<TID, TOriginal, TModified>(TID id, TModified modified, bool transaction = false, CancellationToken ct = default)
        where TOriginal : class, new()
        where TModified : class, new()
        where TID : struct
    {
        IDbContextTransaction? dbTransaction = null;
        try
        {
            if (transaction)
                dbTransaction = await _context.Database.BeginTransactionAsync();
            var table = _context.Set<TOriginal>();

            var item = await table.FirstOrDefaultAsync(x => (x as BaseEntity<TID>).id.Equals(id), ct);
            if (item == null)
                throw new Exception("entity not found");
            item = item.CopyChangesFrom(modified);
            var entry = table.Update(item);
            await _context.SaveChangesAsync(ct);
            if (transaction && dbTransaction != null)
                await dbTransaction.CommitAsync(ct);
            return entry.Entity;
        }
        catch (Exception ex)
        {
            if (transaction && dbTransaction != null)
            {
                await dbTransaction.RollbackAsync(ct);
            }

            throw new Exception("An error occurred while updating the entity.");
        }
    }

    public async Task<(IEnumerable<TEntity>, int)> GetAsync<TQuery, TEntity>(TQuery query, PaginationQuery pagination,
        CancellationToken ct = default) where TEntity : class, new()
    {
        var table = _context.Set<TEntity>().AsQueryable();
        table = table.IncludeAll().DynamicSearch(query);
        int total = await table.CountAsync(ct);
        var result = await table.Skip((pagination.page - 1) * pagination.pageSize)
            .Take(pagination.pageSize).ToListAsync(ct);
        return (result, total);
    }

    public async Task<TEntity> ShowAsync<TID, TEntity>(TID id, CancellationToken ct = default) where TEntity : class, new() where TID : struct
    {
        var table = _context.Set<TEntity>().IncludeAll();

        var item = await table.FirstOrDefaultAsync(x => (x as BaseEntity<TID>).id.Equals(id), ct);
        return item;
    }

    public async Task<TEntity> DeleteAsync<TID, TEntity>(TID id, CancellationToken ct = default) where TEntity : class, new() where TID : struct
    {
        var table = _context.Set<TEntity>();

        var item = await table.FirstOrDefaultAsync(x => (x as BaseEntity<TID>).id.Equals(id), ct);
        if (item == null)
            throw new Exception("entity not found");
        table.SoftRemove(item);
        await _context.SaveChangesAsync(ct);
        return item;
    }
}