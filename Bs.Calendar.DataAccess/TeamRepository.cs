using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class TeamRepository : BaseRepository<Team>, ITeamRepository
    {
        public IQueryable<Team> Load(TeamFilter filter)
        {
            var query = Load()
                .WhereIf(filter.Name.IsNotEmpty(), x => x.Name.Contains(filter.Name));
            
            query = query
                .OrderByExpression(filter.SortByField)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return query;
        }
    }
}
