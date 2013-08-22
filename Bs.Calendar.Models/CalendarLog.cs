using System;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class CalendarLog : BaseEntity
    {
        public LogTypes LogType { get; set; }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
