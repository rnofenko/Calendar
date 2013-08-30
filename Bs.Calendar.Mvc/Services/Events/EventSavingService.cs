using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels.Events;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;

namespace Bs.Calendar.Mvc.Services.Events
{
    public class EventSavingService
    {
        private readonly RepoUnit _unit;

        public EventSavingService(RepoUnit repoUnit)
        {
            _unit = repoUnit;
        }

        public int Save(CalendarEventVm calendarEventVm, int userId)
        {
            if (calendarEventVm.EventType == EventType.Personal) return savePersonalEvent(calendarEventVm, userId);
            if (calendarEventVm.EventType == EventType.Meeting) return saveMeetingEvent(calendarEventVm);
            return 0;
        }

        public int Update(CalendarEventVm calendarEventVm)
        {
            var calendarEvent = calendarEventVm.Map();

            if (calendarEventVm.EventType == EventType.Personal)
            {
                var history = _unit.EmailOnEventHistory.Get(e => e.Event.Id == calendarEvent.Id);
                var user = history.User;
                _unit.EmailOnEventHistory.Delete(history);
                _unit.PersonalEvent.Delete(_unit.PersonalEvent.Get(l => l.Event.Id == calendarEvent.Id));

                calendarEventVm.Id = 0;
                return savePersonalEvent(calendarEventVm, user.Id);
            }
            
            var oldPersonalLinks = _unit.PersonalEvent.Load(link => link.Event.Id == calendarEvent.Id).ToList();
            var oldTeamLinks = _unit.TeamEvent.Load(link => link.Event.Id == calendarEvent.Id).ToList();

            //Clear all deprecated event history records
            _unit.EmailOnEventHistory
                .Load(history => history.Event.Id == calendarEvent.Id)
                .ToList()
                .ForEach(_unit.EmailOnEventHistory.Delete);

            //Clear all deprecated event records
            oldPersonalLinks.ForEach(_unit.PersonalEvent.Delete);
            oldTeamLinks.ForEach(_unit.TeamEvent.Delete);

            //Recreate event
            calendarEventVm.Id = 0;
            return saveMeetingEvent(calendarEventVm);
        }

        private int savePersonalEvent(CalendarEventVm calendarEventVm, int userId)
        {
            var calendarEvent = calendarEventVm.Map();

            if(calendarEvent.Id == 0)
            {
                _unit.PersonalEvent.Save(new PersonalEventLink { Event = calendarEvent, User = _unit.User.Get(userId) });
            }

            saveToEmailHistory(calendarEventVm, calendarEvent, userId);

            return calendarEvent.Id;
        }

        private int saveMeetingEvent(CalendarEventVm calendarEventVm)
        {
            var calendarEvent = calendarEventVm.Map();

            if (calendarEventVm.Users != null)
            {
                calendarEventVm.Users.ForEach(user => _unit.PersonalEvent.Save(new PersonalEventLink { Event = calendarEvent, User = _unit.User.Get(user.UserId) }));
            }

            if (calendarEventVm.Teams != null)
            {
                calendarEventVm.Teams.ForEach(team =>
                    _unit.TeamEvent.Save(new TeamEventLink { Event = calendarEvent, Team = _unit.Team.Get(team.Id) }));
            }

            saveToEmailHistory(calendarEventVm, calendarEvent, 0);

            return calendarEvent.Id;
        }

        private void saveToEmailHistory(CalendarEventVm calendarEventVm, CalendarEvent calendarEvent, int userId)
        {
            var eventUsers = getEventUsers(calendarEventVm);
            if (calendarEvent.EventType == EventType.Personal) eventUsers.Add(_unit.User.Get(userId));

            eventUsers.ForEach(u => _unit.EmailOnEventHistory.Save(new EmailOnEventHistory {Event = calendarEvent, User = u}));
        }

        private List<User> getEventUsers(CalendarEventVm calendarEventVm)
        {
            var eventUsers = new List<User>();

            if (calendarEventVm.Users != null) 
            {
                calendarEventVm.Users.ForEach(user => eventUsers.Add(_unit.User.Get(user.UserId)));
            }

            if (calendarEventVm.Teams != null) 
            {
                calendarEventVm.Teams.ForEach(team => _unit.Team.Get(team.Id)
                               .Users.ToList().ForEach(eventUsers.Add));
            }

            return eventUsers.Distinct().ToList();
        }
    }
}