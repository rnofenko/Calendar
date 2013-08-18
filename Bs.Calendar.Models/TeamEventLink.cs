using Bs.Calendar.Models.Bases;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class TeamEventLink : BaseEntity
    {
        public virtual Team Team { get; set; }

        public virtual CalendarEvent Event { get; set; }
    }
}
