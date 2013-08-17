using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    class FakeTeamRepository : FakeBaseRepository<Team>, ITeamRepository
    {
        public IQueryable<Team> Load(TeamFilter filter)
        {
            var stringComparisonOption = StringComparison.OrdinalIgnoreCase;

            var query = Load()
                .WhereIf(filter.Name.IsNotEmpty(), x => x.Name.Contains(filter.Name, stringComparisonOption));

            query = query
                .OrderByExpression(filter.SortByField)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return query;
        }
    }
}
