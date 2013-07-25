using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public new IQueryable<User> Load()
        {
            return Load(u => u.LiveState == LiveState.Ok);
        }
    }
}
