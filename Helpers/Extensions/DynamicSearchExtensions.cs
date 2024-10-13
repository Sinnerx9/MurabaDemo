using System.Linq.Expressions;
using System.Reflection;
using MurabaDemo.Helpers.Attributes;

namespace MurabaDemo.Helpers.Extensions;

public static class DynamicSearchExtensions
{
    public static IQueryable<TQueryable> DynamicSearch<TFilter, TQueryable>(this IQueryable<TQueryable> queryable, TFilter filter)
    {
        var parameter = Expression.Parameter(typeof(TQueryable), "x");
        var predicate = filter.GetType()
                              .GetProperties()
                              .Select(property => CreateFilterExpression<TQueryable>(parameter, property, filter))
                              .Where(expression => expression != null)
                              .Aggregate<Expression, Expression>(Expression.Constant(true), Expression.AndAlso);

        var lambda = Expression.Lambda<Func<TQueryable, bool>>(predicate, parameter);
        return queryable.Where(lambda);
    }

    private static Expression CreateFilterExpression<TQueryable>(ParameterExpression parameter, PropertyInfo filterProperty, object filter)
    {
        var attribute = filterProperty.GetCustomAttribute<MapToPropertyAttribute>();
        string entityPropertyName = attribute?.EntityPropertyName ?? filterProperty.Name;
    
        var entityProperty = typeof(TQueryable).GetProperty(entityPropertyName);
        if (entityProperty == null) return null;

        var value = filterProperty.GetValue(filter);
        if (value == null) return null;

        var propertyAccess = Expression.Property(parameter, entityProperty);

        if (filterProperty.Name.EndsWith("Start") || filterProperty.Name.EndsWith("End"))
        {
            return GenerateRangeExpression(propertyAccess, filter, entityPropertyName);
        }

        return GenerateExpressionForProperty(propertyAccess, filterProperty.PropertyType, filter, filterProperty.Name);
    }


    private static Expression GenerateExpressionForProperty(Expression propertyAccess, Type propertyType, object filter, string propertyName)
    {
        if (propertyType == typeof(string))
        {
            return GenerateStringContainsExpression(propertyAccess, filter, propertyName);
        }

        if (IsRangeSupported(propertyType))
        {
            return GenerateRangeExpression(propertyAccess, filter, propertyName);
        }

        return GenerateEqualityExpression(propertyAccess, filter, propertyName);
    }

    private static Expression GenerateStringContainsExpression(Expression propertyAccess, object filter, string propertyName)
    {
        var value = filter.GetType().GetProperty(propertyName)?.GetValue(filter)?.ToString();
        if (string.IsNullOrEmpty(value)) return null;

        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        var valueExpression = Expression.Constant(value.ToLower());
        var propertyToLower = Expression.Call(propertyAccess, toLowerMethod);
        return Expression.Call(propertyToLower, containsMethod, valueExpression);
    }

    private static Expression GenerateRangeExpression(Expression propertyAccess, object filter, string propertyName)
    {
        var startValue = filter.GetType().GetProperty($"{propertyName}Start")?.GetValue(filter);
        var endValue = filter.GetType().GetProperty($"{propertyName}End")?.GetValue(filter);

        Expression predicate = Expression.Constant(true);

        if (startValue != null)
        {
            var startExpression = Expression.GreaterThanOrEqual(propertyAccess, Expression.Constant(startValue));
            predicate = Expression.AndAlso(predicate, startExpression);
        }

        if (endValue != null)
        {
            var endExpression = Expression.LessThanOrEqual(propertyAccess, Expression.Constant(endValue));
            predicate = Expression.AndAlso(predicate, endExpression);
        }

        return predicate;
    }

    private static Expression GenerateEqualityExpression(Expression propertyAccess, object filter, string propertyName)
    {
        var value = filter.GetType().GetProperty(propertyName)?.GetValue(filter);
        if (value == null) return null;

        return Expression.Equal(propertyAccess, Expression.Constant(value));
    }

    private static bool IsRangeSupported(Type type)
    {
        return type == typeof(DateTime) || type.IsNumericType() || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(type).IsNumericType());
    }

    private static bool IsNumericType(this Type type)
    {
        return type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(decimal) ||
               type == typeof(long) || type == typeof(short) || type == typeof(byte) || type == typeof(uint) ||
               type == typeof(ulong) || type == typeof(ushort) || type == typeof(sbyte);
    }
}
