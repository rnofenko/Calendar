using System;
using System.Linq;
using System.Linq.Expressions;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.DataAccess.Bases
{
    public interface IRepository<T> : IDisposable, IContexable where T : IEntityId
    {
        IQueryable<T> Load();
        IQueryable<T> Load(Expression<Func<T, bool>> predicate);
        T Get(Expression<Func<T, bool>> predicate);        
        T Get(int id);
        
        void Delete(T entity);
        
        void Save(T entity);
    }
}