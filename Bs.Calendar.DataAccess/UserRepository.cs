using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(CalendarContext context)
            :base(context)
        {
        }
    }
}