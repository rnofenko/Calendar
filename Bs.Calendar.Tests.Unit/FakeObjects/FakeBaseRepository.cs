using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    public class FakeBaseRepository<T> : IRepository<T> where T : class, IEntityId
    {
        private static int _id;
        private readonly List<T> _entities = new List<T>();

        public void Dispose()
        {
            _entities.Clear();
        }

        public virtual IQueryable<T> Load()
        {
            return _entities.AsQueryable();
        }

        public IQueryable<T> Load(Expression<Func<T, bool>> predicate)
        {
            return _entities.AsQueryable().Where(predicate);
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return _entities.AsQueryable().FirstOrDefault(predicate);
        }

        public T Get(int id)
        {
            return _entities.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(T entity)
        {
            var fromCollection = Get(entity.Id);
            if (fromCollection != null)
            {
                _entities.Remove(fromCollection);
            }
        }

        public virtual void Save(T entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = ++_id;
                _entities.Add(entity);
            }
            else
            {
                Delete(entity);
                _entities.Add(entity);
            }
        }

        public void SetContext(object context)
        {
        }
    }
}