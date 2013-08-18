using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bs.Calendar.Models
{
    public class TeamEventLink
    {
        public Team Team { get; set; }
        public CalendarEvent Event { get; set; }
    }
}
