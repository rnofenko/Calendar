using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public interface ITeamEventRepository : IRepository<TeamEventLink>
    {
        IQueryable<TeamEventLink> Load(MeetingEventFilter filter);
    }
}
