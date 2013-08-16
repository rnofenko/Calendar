using System;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Core;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    public class FakeUserRepository : FakeBaseRepository<User>, IUserRepository
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
                    query = query.Where(x => x.Email.Contains(name, StringComparison.OrdinalIgnoreCase) || x.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
                }
            }

            query
                .WhereIf(filter.Roles > 0 && filter.Roles < Roles.All, x => (x.Role & filter.Roles) > 0)
                .WhereIf(filter.LiveStatuses > 0 && filter.LiveStatuses < LiveStatuses.All,
                         x => (x.Live & filter.LiveStatuses) > 0)
                .WhereIf(filter.ApproveStates > 0 && filter.ApproveStates < ApproveStates.All,
                         x => (x.ApproveState & filter.ApproveStates) > 0);

            query = query
                .OrderByExpression(filter.SortByField)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return query;
        }
    }
}
