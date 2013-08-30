using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    class FakeTeamEventRepository : FakeBaseRepository<TeamEventLink>, ITeamEventRepository
    {
        public IQueryable<TeamEventLink> Load(MeetingEventFilter filter)
        {
            return Load()
                .Where(link => link.Event.DateStart >= filter.FromDate && link.Event.DateEnd <= filter.ToDate);
        }
    }
}
