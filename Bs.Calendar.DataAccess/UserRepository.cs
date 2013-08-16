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
                foreach (var name in filter.EmailOrFullName.Where(x => x.IsNotEmpty()))
                {
                    query = query.Where(x => x.Email.Contains(name) || x.FullName.Contains(name));
                }
            }

            query
                .WhereIf(filter.Roles > 0 && filter.Roles < Roles.All, x => (x.Role & filter.Roles) > 0)
                .WhereIf(filter.LiveStatuses > 0 && filter.LiveStatuses < LiveStatuses.All, x => (x.Live & filter.LiveStatuses) > 0)
                .WhereIf(filter.ApproveStates.IsNotAllOrNull(), x => (x.ApproveState & filter.ApproveStates) > 0);

            query = query
                .OrderByExpression(filter.SortByField)
                .Skip(filter.Page - 1)
                .Take(filter.PageSize);

            return query;
        }
    }
}
