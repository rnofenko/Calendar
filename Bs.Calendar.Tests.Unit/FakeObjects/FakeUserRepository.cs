using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    public class FakeUserRepository : FakeBaseRepository<User>, IUserRepository
    {
        public override IQueryable<User> Load() {
            return Load(u => u.LiveState == LiveState.Active);
        }
    }
}
