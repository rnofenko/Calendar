using System;
using System.Linq;
using System.Linq.Expressions;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class CalendarLogRepository : BaseRepository<CalendarLog>, ICalendarLogRepository
    {
    }
}