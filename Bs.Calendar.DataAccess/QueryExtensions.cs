using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bs.Calendar.DataAccess
{
    public static partial class QueryExtensions
    {
        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            return source;
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
    }
}
