using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class PersonalEventRepository : BaseRepository<PersonalEventLink>, IPersonalEventRepository
    {
        public IQueryable<PersonalEventLink> Load(PersonalEventFilter filter)
        {
            return Load()
                .Where(link => link.Event.DateStart >= filter.FromDate && link.Event.DateEnd <= filter.ToDate);
        }
    }
}
