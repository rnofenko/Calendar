using Bs.Calendar.Models.Bases;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class PersonalEventLink : BaseEntity
    {
        public virtual User User { get; set; }

        public virtual CalendarEvent Event { get; set; } 

        public int EventStatus { get; set; }
    }
}
