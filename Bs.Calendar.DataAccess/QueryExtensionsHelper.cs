using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bs.Calendar.DataAccess
{
    public static partial class QueryExtensions
    {
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
