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

        public CalendarVm RetreiveList(EventFilterVm filter, string user)
        {
            var  userEntity = _unit.User.Get(u => u.Email == user);

            MeetingEventFilter meetingEventFilter;
            PersonalEventFilter personalEventFilter;

            filter.Map(out meetingEventFilter);
            filter.Map(out personalEventFilter);

            var calendarEvents = _unit.PersonalEvent
                .Load(personalEventFilter)
                .Where(link => link.User.Id == userEntity.Id)
                .Select(e => new CalendarCellEventVm(e.Event))
                .Union(_unit.TeamEvent
                    .Load(meetingEventFilter)
                    .Where(link => link.Team.Live == LiveStatuses.Active && link.Team.Users.Select(u=>u.Id).Contains(userEntity.Id))
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

        public int Save(CalendarCellEventVm calendarEvent, string currentUserEmail)
        {
            var currentUser = _unit.User.Get(user => user.Email == currentUserEmail);
            if(currentUser != null)
            {
                return _savingService.Save(new CalendarEventVm(calendarEvent.Map()), currentUser.Id);
            }

            return 0;
        }

        public CalendarEvent GetEvent(int id)
        {
            var personalEventLink = _unit.PersonalEvent.Get(link => link.Event.Id == id);
            var teamEventLink = _unit.TeamEvent.Get(link => link.Event.Id == id);

            var calendarEvent = personalEventLink == null ? null : personalEventLink.Event;
            calendarEvent = calendarEvent ?? (teamEventLink == null ? null : teamEventLink.Event);

            return calendarEvent;
        }

        public void Delete(CalendarCellEventVm calendarEvent)
        {
            if(calendarEvent.EventType == EventType.Personal)
            {
                var toDelete = _unit.PersonalEvent.Get(l => l.Event.Id == calendarEvent.Id);

                if(toDelete != null)
                {
                    _unit.PersonalEvent.Delete(toDelete);
                }
            }
            else
            {
                var toDelete = _unit.TeamEvent.Get(l => l.Event.Id == calendarEvent.Id);

                if (toDelete != null)
                {
                    _unit.TeamEvent.Delete(toDelete);
                }
            }
        }
    }
}