using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;

namespace Bs.Calendar.Mvc.ViewModels.Home
{
    public class CalendarEventVm
    {
        public CalendarEvent Map(CalendarEventVm calendarEvent)
        {
            return new CalendarEvent
            {
                Id = Id,
                Title = Title,
                Text = Text,
                DateStart = DateStart,
                DateEnd = DateEnd,
                EventType = EventType,
                Room = Room
            };
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required!"),
        StringLength(BaseEntity.LENGTH_NAME),
        Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required!"),
        Display(Name = "Description")]
        public string Text { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public Room Room { get; set; }
        public EventType EventType { get; set; }

        public List<UserVm> Users { get; set; }
        public List<TeamVm> Teams { get; set; }
    }
}