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
            var query = Load()
                .WhereIf(filter.Email.IsNotEmpty(), x => x.Email.Contains(filter.Email))
                .WhereIf(filter.Name.IsNotEmpty(), x => x.FullName.Contains(filter.Name))
                .WhereIf(filter.SecondName.IsNotEmpty(), x => x.FullName.Contains(filter.SecondName))
                .WhereIf(filter.Roles > 0 && filter.Roles < Roles.All, x => (x.Role & filter.Roles) > 0)
                .WhereIf(filter.LiveStatuses > 0 && filter.LiveStatuses < LiveStatuses.All, x => (x.Live & filter.LiveStatuses) > 0)
                .WhereIf(filter.ApproveStates > 0 && filter.ApproveStates < ApproveStates.All, x => (x.ApproveState & filter.ApproveStates) > 0);

            query = query
                .OrderByExpression(filter.SortByField)
                .Skip(filter.Page - 1)
                .Take(filter.PageSize);

            return query;
        }
    }
}
