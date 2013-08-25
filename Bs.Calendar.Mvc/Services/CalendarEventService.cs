using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels.Home;

namespace Bs.Calendar.Mvc.Services
{
    public class CalendarEventService
    {
        private readonly RepoUnit _unit;

        public CalendarEventService(RepoUnit repoUnit)
        {
            _unit = repoUnit;
        }

        public void Save(CalendarEventVm calendarEventVm, int userId)
        {
            if (calendarEventVm.EventType == EventType.Personal) savePersonalEvent(calendarEventVm, userId);
            if (calendarEventVm.EventType == EventType.Meeting) saveMeetingEvent(calendarEventVm);

            saveToEmailHistory(calendarEventVm, userId);
        }

        private void savePersonalEvent(CalendarEventVm calendarEventVm, int userId)
        {
            _unit.PersonalEvent.Save( new PersonalEventLink {Event = calendarEventVm.Map(), User = _unit.User.Get(userId)});
        }

        private void saveMeetingEvent(CalendarEventVm calendarEventVm)
        {
            var calendarEvent = calendarEventVm.Map();
            
            if (calendarEventVm.Users != null)
            {
                calendarEventVm.Users.ForEach(user =>
                    _unit.PersonalEvent.Save(new PersonalEventLink { Event = calendarEvent, User = _unit.User.Get(user.UserId) }));
            }

            if (calendarEventVm.Teams != null)
            {
                calendarEventVm.Teams.ForEach(team =>
                    _unit.TeamEvent.Save(new TeamEventLink { Event = calendarEvent, Team = _unit.Team.Get(team.Id) }));
            }
        }

        private void saveToEmailHistory(CalendarEventVm calendarEventVm, int userId)
        {
            var emailOnEventHistory = createEmailOnEventHistory(calendarEventVm, userId);
            emailOnEventHistory.ForEach(h => _unit.EmailOnEventHistory.Save(h));
        }

        private List<EmailOnEventHistory> createEmailOnEventHistory(CalendarEventVm calendarEventVm, int userId)
        {
            var emailOnEventHistory = new List<EmailOnEventHistory>();
            var calendarEvent = calendarEventVm.Map();

            if (calendarEventVm.EventType == EventType.Personal)
            {
                emailOnEventHistory.Add(new EmailOnEventHistory { Event = calendarEvent, User = _unit.User.Get(userId) });
            }

            if (calendarEventVm.EventType == EventType.Meeting)
            {
                if (calendarEventVm.Users != null)
                {
                    calendarEventVm.Users.ForEach(user =>
                        emailOnEventHistory.Add(new EmailOnEventHistory { Event = calendarEvent, User = _unit.User.Get(user.UserId) }));
                }

                if (calendarEventVm.Teams != null)
                {
                    calendarEventVm.Teams.ForEach(team => _unit.Team.Get(team.Id)
                                   .Users.ToList().ForEach(user => emailOnEventHistory.Add(new EmailOnEventHistory { Event = calendarEvent, User = user })));
                }
            }

            return emailOnEventHistory.Distinct().ToList();
        }
    }
}