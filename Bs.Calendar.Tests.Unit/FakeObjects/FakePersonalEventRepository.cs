using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    public class FakePersonalEventRepository : FakeBaseRepository<PersonalEventLink>, IPersonalEventRepository
    {
        public IQueryable<PersonalEventLink> Load(PersonalEventFilter filter)
        {
            return Load()
                .Where(link => link.Event.DateStart >= filter.FromDate && link.Event.DateEnd <= filter.ToDate);
        }
    }
}
