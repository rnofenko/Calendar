using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    public class FakeUserRepository : FakeBaseRepository<User>, IUserRepository
    {
        public override IQueryable<User> Load() {
            //return Load(u => u.Live == LiveStatuses.Active);
            return base.Load();
        }

        public override void Save(User entity)
        {
            entity.FullName = string.Format("{0} {1}", entity.FirstName, entity.LastName);
            base.Save(entity);
        }

        public IQueryable<User> Load(UserFilter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}
