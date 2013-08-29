using System;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Home
{
    public class EventFilterVm
    {
        public void Map(out PersonalEventFilter filter)
        {
            filter = new PersonalEventFilter
                         {
                             FromDate = FromDate,
                             ToDate = ToDate
                         };
        }

        public void Map(out MeetingEventFilter filter)
        {
            filter = new MeetingEventFilter
                         {
                             FromDate = FromDate,
                             ToDate = ToDate
                         };
        }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}