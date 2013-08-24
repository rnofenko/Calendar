using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class EmailOnEventHistory : BaseEntity 
    {
        public virtual CalendarEvent Event { get; set; }
        public virtual User User { get; set; }
        public EmailOnEventStatus EmailOnEventStatus { get; set; }
    }
}
