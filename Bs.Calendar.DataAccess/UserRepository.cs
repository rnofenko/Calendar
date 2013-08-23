using System;
using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Core;

namespace Bs.Calendar.DataAccess
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public override void Save(User entity)
        {
            entity.FullName = string.Format("{0} {1}", entity.FirstName, entity.LastName);
            base.Save(entity);
        }

        public IQueryable<User> Load(UserFilter filter)
        {
            var query = Load();

            if (filter.EmailOrFullName != null)
            {
                foreach (var name in filter.EmailOrFullName)
                {
                    query = query.Where(x => x.Email.Contains(name) || x.FullName.Contains(name));
                }
            }

            query = query
                .WhereIf(filter.Roles > 0 && filter.Roles < Roles.All, x => (x.Role & filter.Roles) > 0)
                .WhereIf(filter.LiveStatuses > 0 && filter.LiveStatuses < LiveStatuses.All,
                         x => (x.Live & filter.LiveStatuses) > 0)
                .WhereIf(filter.ApproveStates > 0 && filter.ApproveStates < ApproveStates.All,
                         x => (x.ApproveState & filter.ApproveStates) > 0);

            query = query
                .OrderByExpression(filter.SortByField);

            if(OnBeforePaging != null)
            {
                OnBeforePaging.Invoke(query);
            }
                
            query = query
                    .Skip((filter.Page - 1)*filter.PageSize)
                    .Take(filter.PageSize);

            return query;
        }

        public event Action<IQueryable<User>> OnBeforePaging;
    }
}
