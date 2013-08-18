using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class TeamEventLink : BaseEntity
    {
        public Team Team { get; set; }
        public CalendarEvent Event { get; set; }
    }
}
