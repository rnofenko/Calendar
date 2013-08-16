using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bs.Calendar.Core
{
    public static class QueryExtensions
    {
        public static IOrderedQueryable<TEntity> OrderByExpression<TEntity>(this IQueryable<TEntity> source, string sortExpression) where TEntity : class
        {
            var orderFields = sortExpression.Split(',');
            IOrderedQueryable<TEntity> result = null;
            for (var currentFieldIndex = 0; currentFieldIndex < orderFields.Length; currentFieldIndex++)
            {
                var expressionPart = orderFields[currentFieldIndex].Trim().Split(' ');
                var sortField = expressionPart[0];
                var sortDescending = (expressionPart.Length == 2) && (expressionPart[1].Equals("DESC", StringComparison.OrdinalIgnoreCase));
                if (sortDescending)
                {
                    result = currentFieldIndex == 0 ? source.orderByDescending(sortField) : result.thenByDescending(sortField);
                }
                else
                {
                    result = currentFieldIndex == 0 ? source.orderBy(sortField) : result.thenBy(sortField);
                }
            }
            return result;
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate) {
            if (condition)
                return source.Where(predicate);
            return source;
        }

        public static IQueryable<TSource> OrderByIf<TSource, TKey>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, TKey>> predicate) {
            if (condition)
                return source.OrderBy(predicate);
            return source;
        }

        public static IQueryable<TSource> OrderByDescIf<TSource, TKey>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, TKey>> predicate) {
            if (condition)
                return source.OrderByDescending(predicate);
            return source;
        }

        private static IOrderedQueryable<TEntity> orderBy<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = generateMethodCall(source, "OrderBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        private static IOrderedQueryable<TEntity> orderByDescending<TEntity>(this IQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = generateMethodCall(source, "OrderByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        private static IOrderedQueryable<TEntity> thenBy<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = generateMethodCall(source, "ThenBy", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }
        private static IOrderedQueryable<TEntity> thenByDescending<TEntity>(this IOrderedQueryable<TEntity> source, string fieldName) where TEntity : class
        {
            var resultExp = generateMethodCall(source, "ThenByDescending", fieldName);
            return source.Provider.CreateQuery<TEntity>(resultExp) as IOrderedQueryable<TEntity>;
        }

        private static MethodCallExpression generateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, String fieldName) where TEntity : class
        {
            var type = typeof(TEntity);
            Type selectorResultType;
            var selector = generateSelector<TEntity>(fieldName, out selectorResultType);
            var resultExp = Expression.Call(typeof(Queryable), methodName, new[] { type, selectorResultType },
                            source.Expression, Expression.Quote(selector));
            return resultExp;
        }
        private static LambdaExpression generateSelector<TEntity>(String propertyName, out Type resultType) where TEntity : class
        {
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields.
                var childProperties = propertyName.Split('.');
                property = typeof(TEntity).GetProperty(childProperties[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (var i = 1; i < childProperties.Length; i++)
                {
                    property = property.PropertyType.GetProperty(childProperties[i], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = typeof(TEntity).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }
    }
}
