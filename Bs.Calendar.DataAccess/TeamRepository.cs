using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class TeamRepository : BaseRepository<Team>
    {
        public TeamRepository(CalendarContext context) : base(context)
        {
        }
    }
}
