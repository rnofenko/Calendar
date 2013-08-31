using System;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels.Home
{
    public class CalendarCellEventVm
    {
        public CalendarEvent Map()
        {
            return new CalendarEvent
                       {
                           Id = Id,
                           Title = Title,
                           DateStart = DateStart,
                           DateEnd = DateEnd,
                           EventType = EventType,
                           RoomId = Room == null ? null : (int?)Room.Id,
                           IsAllDay = IsAllDay
                       };
        }

        public CalendarCellEventVm(CalendarEvent eventInstance)
        {
            Id = eventInstance.Id;
            Title = eventInstance.Title;

            DateStart = eventInstance.DateStart;
            DateEnd = eventInstance.DateEnd;
            Text = eventInstance.Text;
            IsAllDay = eventInstance.IsAllDay;
            Room = eventInstance.Room;

            EventType = eventInstance.EventType;
        }

        public CalendarCellEventVm()
        {
            EventType = EventType.Personal;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required!"),
         StringLength(BaseEntity.LENGTH_NAME),
         Display(Name = "Title")]
        public string Title { get; set; }
        public string Text { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public bool IsAllDay { get; set; }

        public Room Room { get; set; }
        public EventType EventType { get; set; }
    }
}