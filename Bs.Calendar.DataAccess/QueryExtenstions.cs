using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bs.Calendar.DataAccess
{
    public static class QueryExtenstions
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
    }
}
