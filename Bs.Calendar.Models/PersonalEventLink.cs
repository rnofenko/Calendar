using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class PersonalEventLink : BaseEntity
    {
        public virtual User Users { get; set; }

        public virtual CalendarEvent Event { get; set; }

        public int EventStatus { get; set; }
    }
}
