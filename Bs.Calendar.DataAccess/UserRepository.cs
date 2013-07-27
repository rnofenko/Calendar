using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public override IQueryable<User> Load()
        {
            return Load(u => u.LiveState == LiveState.Ok);
        }

        public override void Save(User entity)
        {
            entity.FullName = string.Format("{0} {1}", entity.FirstName, entity.LastName);
            base.Save(entity);
        }
    }
}
