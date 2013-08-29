using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services.Events;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Events;
using Bs.Calendar.Mvc.ViewModels.Home;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class HomeService
    {
        private readonly UsersRules _rules;
        private readonly RepoUnit _unit;
        private readonly EventSavingService _savingService;

        public HomeService(UsersRules rules, RepoUnit unit, EventSavingService calendarEventService)
        {
            _savingService = calendarEventService;
            _rules = rules;
            _unit = unit;
            var users = unit.User.Load();
            if (!users.Any())
            {
                var defaultUser = new User
                {
                    BirthDate = null,
                    Email = "Admin",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Role = Roles.Admin,
                    PasswordHash = "Admin",
                    PasswordSalt = "",

                    Live = LiveStatuses.Active,
                    ApproveState = ApproveStates.Approved
                };
                unit.User.Save(defaultUser);
            }
        }

        public CalendarVm RetreiveList(EventFilterVm filter)
        {
            MeetingEventFilter meetingEventFilter;
            PersonalEventFilter personalEventFilter;

            filter.Map(out meetingEventFilter);
            filter.Map(out personalEventFilter);

            var calendarEvents = _unit.PersonalEvent
                .Load(personalEventFilter)
                .Select(e => new CalendarCellEventVm(e.Event))
                .Concat(_unit.TeamEvent
                    .Load(meetingEventFilter)
                    .Select(e => new CalendarCellEventVm(e.Event)));

            var birthdays = _rules
                .LoadUsersByBirthday(filter.FromDate, filter.ToDate)
                .Select(u => new BirthdayEventVm {Date = u.BirthDate.Value, Text = u.LastName});

            return new CalendarVm
                       {
                           BirthdayEvents = birthdays,
                           CalendarEvents = calendarEvents
                       };
        }
        public void Save(CalendarCellEventVm calendarEvent, string currentUserEmail)
        {
            var currentUser = _unit.User.Get(user => user.Email == currentUserEmail);
            if(currentUser != null)
            {
                _savingService.Save(new CalendarEventVm(calendarEvent.Map()), currentUser.Id);
            }
        }

        public CalendarEvent GetEvent(int id, EventType type)
        {
            if(type == EventType.Personal)
            {
                var link = _unit.PersonalEvent.Get(id);
                return link == null ? link.Event : null;
            }
            else
            {
                var link = _unit.TeamEvent.Get(id);
                return link == null ? link.Event : null;
            }
        }
    }
}