using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public interface IPersonalEventRepository : IRepository<PersonalEventLink>
    {
        IQueryable<PersonalEventLink> Load(PersonalEventFilter filter);
    }
}
