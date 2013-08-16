using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public interface ITeamRepository : IRepository<Team>
    {
        IQueryable<Team> Load(TeamFilter filter);
    }
}
