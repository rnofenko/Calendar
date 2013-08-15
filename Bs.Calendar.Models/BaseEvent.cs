using System;

namespace Bs.Calendar.Models
{
    public class BaseEvent
    {
        public string Theme { get; set; }
        public string Text { get; set; }
        public Room Place { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public bool IsAllDay { get; set; }
    }
}
