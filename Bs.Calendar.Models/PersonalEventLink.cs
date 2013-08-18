using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bs.Calendar.Models
{
    public class PersonalEventLink
    {
        public virtual User Users { get; set; }

        public virtual CalendarEvent Event { get; set; }

        public EventStatus EventStatus { get; set; }
    }
}
