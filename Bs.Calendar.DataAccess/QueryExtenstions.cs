using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
