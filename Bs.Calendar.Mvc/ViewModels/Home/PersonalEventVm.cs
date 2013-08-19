using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels.Home
{
    public class PersonalEventVm
    {
        public PersonalEventVm()
        {
            
        }

        public PersonalEventVm(CalendarEvent calendarEvent)
        {
            EventId = calendarEvent.Id;

            Title = calendarEvent.Title;
            Text = calendarEvent.Text;
            
            DateStart = calendarEvent.DateStart;
            DateEnd = calendarEvent.DateEnd;
            IsAllDay = calendarEvent.IsAllDay;
        }

        public CalendarEvent Map(PersonalEventVm personalEvent)
        {
            return new CalendarEvent
                {
                    Id = EventId,
                    Title = Title,
                    Text = Text,
                    DateStart = DateStart,
                    DateEnd = DateEnd,
                    IsAllDay = IsAllDay,
                    EventType = EventType.Personal,
                    Room = null
                };
        }

        public int EventId { get; set; }

        [Required(ErrorMessage = "Title is required!"),
        StringLength(BaseEntity.LENGTH_NAME),
        Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required!"),
        Display(Name = "Description")]
        public string Text { get; set; }

        [Required(ErrorMessage = "From Date is required!"),
        Display(Name = "Date"),
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = "Date is required!"),
        Display(Name = "Date"),
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateEnd { get; set; }

        [Display(Name = "Don't consider time")]
        public bool IsAllDay { get; set; }
    }
}