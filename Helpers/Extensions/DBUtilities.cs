using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MurabaDemo.Models.Infrastructure;

namespace MurabaDemo.Helpers.Extensions;

public static class DBUtilities
{
    public static IQueryable<TEntity> IncludeAll<TEntity>(this IQueryable<TEntity> queryable,
        int maxDepth = int.MaxValue, bool addSeenTypesToIgnoreList = true, HashSet<Type>? ignoreTypes = null)
        where TEntity : class
    {
        var type = typeof(TEntity);
        var includes = new List<string>();
        ignoreTypes ??= new HashSet<Type>();
        GetIncludeTypes(ref includes, prefix: string.Empty, type, ref ignoreTypes, addSeenTypesToIgnoreList,
            maxDepth);
        foreach (var include in includes)
        {
            queryable = queryable.Include(include);
        }

        return queryable;
    }

    private static void GetIncludeTypes(ref List<string> includes, string prefix, Type type,
        ref HashSet<Type> ignoreSubTypes,
        bool addSeenTypesToIgnoreList = true, int maxDepth = int.MaxValue)
    {
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var notMappedAttributes = property.GetCustomAttributes<NotMappedAttribute>().ToList();
            if (notMappedAttributes != null && notMappedAttributes.Count() != 0)
            {
                continue;
            }

            var getter = property.GetGetMethod();
            if (getter != null)
            {
                var isVirtual = getter.IsVirtual;

                if (isVirtual)
                {
                    var propPath = (prefix + "." + property.Name).TrimStart('.');
                    if (maxDepth <= propPath.Count(c => c == '.'))
                    {
                        break;
                    }

                    includes.Add(propPath);

                    var subType = property.PropertyType;
                    if (ignoreSubTypes.Contains(subType))
                    {
                        continue;
                    }
                    else if (addSeenTypesToIgnoreList)
                    {
                        ignoreSubTypes.Add(type);
                    }

                    var isEnumerableType = subType.GetInterface(nameof(IEnumerable)) != null;
                    var genericArgs = subType.GetGenericArguments();
                    if (isEnumerableType && genericArgs.Length == 1)
                    {
                        var subTypeCollection = genericArgs[0];
                        if (subTypeCollection != null)
                        {
                            GetIncludeTypes(ref includes, propPath, subTypeCollection, ref ignoreSubTypes,
                                addSeenTypesToIgnoreList, maxDepth);
                        }
                    }
                    else
                    {
                        GetIncludeTypes(ref includes, propPath, subType, ref ignoreSubTypes,
                            addSeenTypesToIgnoreList, maxDepth);
                    }
                }
            }
        }
    }
    
    public static void SoftRemove<T>(this DbSet<T> context, T entity) where T : class
    {
        Type type = typeof(T);
        PropertyInfo? property = type.GetProperty("isDeleted");
        if (property != null)
        {
            property.SetValue(entity, true);
        }

        context.Update(entity);
    }

    public static void SoftRemoveRange<T>(this DbSet<T> context, IEnumerable<T> entities) where T : class
    {
        for (int i = 0; i < entities.Count(); i++)
        {
            SoftRemove(context, entities.ElementAt(i));
        }
    }

    public static void SetQueryFilters(this ModelBuilder modelBuilder,
        Func<IMutableEntityType, LambdaExpression?> filterFactory)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var filter = filterFactory(entityType);
            if (filter != null)
            {
                var methodInfo = typeof(DBUtilities)
                    .GetMethod(nameof(SetQueryFilter), new[] { typeof(ModelBuilder), typeof(LambdaExpression) });
                if (methodInfo != null)
                {
                    methodInfo.MakeGenericMethod(entityType.ClrType)
                        .Invoke(null, new object[] { modelBuilder, filter });
                }
            }
        }
    }

    public static void SetQueryFilter<TEntity>(this ModelBuilder modelBuilder,
        Expression<Func<TEntity, bool>> filter)
        where TEntity : class
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(filter);
    }

    public static async Task<Response<TEntity>> ToResponseAsync<TEntity>(
        this IQueryable<TEntity> entity)
        where TEntity : class
    {
        var entities = (await entity.FirstOrDefaultAsync());
        if (entities != null)
        {
            return new Response<TEntity>()
            {
                result = entities,
                hasError = false,
                statusCode = 200,
            };
        }
        else
        {
            return new Response<TEntity>()
            {
                result = (TEntity)Activator.CreateInstance(typeof(TEntity)),
                hasError = true,
                errors = ["Entity not found"],
                statusCode = 404,
            };
        }
    }

    
    public static async Task<PaginatedResponse<TResult>> ToPaginatedResponseAsync<TEntity, TResult>(
        this IQueryable<TEntity> entity, 
        PaginationQuery? pagination,
        
        bool mapToDto = false,
        CancellationToken ct = default)
        where TEntity : class
        where TResult : class, new()
    {
        var total = await entity.CountAsync(ct);

        if (pagination == null)
        {
            pagination = new PaginationQuery
            {
                page = 1,
                pageSize = total
            };
        }

        var entities = await entity.Skip((pagination.page - 1) * pagination.pageSize)
            .Take(pagination.pageSize)
            .ToListAsync(ct);

        List<TResult> result;

        if (mapToDto)
        {
            result = entities.Select(e => e.MapTo<TResult>()).Cast<TResult>().ToList();
        }
        else
        {
            result = entities.Cast<TResult>().ToList();
        }

        if (result.Any())
        {
            return new PaginatedResponse<TResult>()
            {
                result = result,
                total = total,
                hasError = false,
                page = pagination.page,
                pageSize = pagination.pageSize,
                statusCode = 200
            };
        }
        else
        {
            return new PaginatedResponse<TResult>()
            {
                result = new List<TResult>(),
                total = total,
                hasError = true,
                page = pagination.page,
                pageSize = pagination.pageSize,
                statusCode = 404
            };
        }
    }
}