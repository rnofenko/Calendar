using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class TeamEventRepository : BaseRepository<TeamEventLink>, ITeamEventRepository
    {
        public IQueryable<TeamEventLink> Load(MeetingEventFilter filter)
        {
            return Load()
                .Where(link => link.Event.DateStart >= filter.FromDate && link.Event.DateEnd <= filter.ToDate);
        }
    }
}
