using System;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class CalendarEvent : BaseEntity
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsAllDay { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }
        public EventType EventType { get; set; }
    }
}
