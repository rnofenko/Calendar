using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Rules
{
    public class EventRules
    {
        private readonly RepoUnit _unit;

        public EventRules(RepoUnit unit)
        {
            _unit = unit;
        }

        public IEnumerable<CalendarEvent> LoadEvents(DateTime from, DateTime to, EventType eventType = EventType.All)
        {
            return null;
        }

        public int NormalizeDate(DateTime date)
        {
            return date.Month * 100 + date.Day;
        }
    }
}