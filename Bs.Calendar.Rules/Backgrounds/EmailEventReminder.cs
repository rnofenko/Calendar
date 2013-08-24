using System;
using System.Collections.Generic;
using Bs.Calendar.DataAccess;
using System.Linq;
using Bs.Calendar.Models;
using Bs.Calendar.Rules.Emails;

namespace Bs.Calendar.Rules.Backgrounds
{
    public class EmailEventReminder : IBackgroundProcess
    {
        private readonly RepoUnit _unit;
        private readonly EmailSender _emailSender;

        private readonly TimeSpan _repeatPeriod = new TimeSpan(0, 1, 0);
        private readonly TimeSpan _timeBeforeReminder = new TimeSpan(3, 0, 0); 
        private readonly TimeSpan _timeBeforeAllDayReminder = new TimeSpan(24, 0, 0);

        public EmailEventReminder(RepoUnit repoUnit, EmailSender emailSender)
        {
            _unit = repoUnit;
            _emailSender = emailSender;
        }

        public void Start()
        {
            var unsentEmailHistory = getNotSentEmailHistory();
            var groupedUserEvents = getGroupedUserEvents(unsentEmailHistory);

            sendEmail(groupedUserEvents);
            saveEmailedEvents(unsentEmailHistory);
        }

        private List<EmailOnEventHistory> getNotSentEmailHistory()
        {
            var notSentEmailHistory = _unit.EmailOnEventHistory.Load(h => h.EmailOnEventStatus == EmailOnEventStatus.NotSent).ToList();
            return notSentEmailHistory.Where(h => eventApproprieteForSend(h.Event)).ToList();
        }

        private List<IGrouping<CalendarEvent, User>> getGroupedUserEvents(IEnumerable<EmailOnEventHistory> emailHistory)
        {
            return emailHistory.GroupBy(h => h.Event, h => h.User).ToList();
        }


        private void saveEmailedEvents(List<EmailOnEventHistory> emailHistory)
        {
            emailHistory.ForEach(h =>
            {
                h.EmailOnEventStatus = EmailOnEventStatus.Sent;
                _unit.EmailOnEventHistory.Save(h);
            });
        }


        private void sendEmail(List<IGrouping<CalendarEvent, User>> groupedEvents) 
        {
            groupedEvents.ForEach(groupedEvent => _emailSender
                .Send(getEmailSubject(groupedEvent.Key), getEmailBody(groupedEvent.Key), groupedEvent.Select(u => u.Email)));
        }

        private string getEmailSubject(CalendarEvent calendarEvent)
        {
            return string.Format("Calendar Event: {0}", calendarEvent.Title);
        }


        private string getEmailBody(CalendarEvent calendarEvent)
        {
            return string.Format(
                "Notification on {0} Event:\n" +
                "Title: {1}\n" +
                "Time: {2}\n" +
                "Event Text: {3}\n",
                Enum.GetName(typeof(EventType), calendarEvent.EventType), calendarEvent.Title, getEventTimeString(calendarEvent), calendarEvent.Text);
        }

        private string getEventTimeString(CalendarEvent calendarEvent)
        {
            if (calendarEvent.IsAllDay) return string.Format("{0} All Day", calendarEvent.DateStart.ToShortDateString());

            if (calendarEvent.EventType == EventType.Personal) return calendarEvent.DateStart.ToString();

            if (calendarEvent.EventType == EventType.Meeting)
            {
                return string.Format("{0} from {1} to {2}", calendarEvent.DateStart.ToShortDateString(),
                    calendarEvent.DateStart.ToShortTimeString(), calendarEvent.DateEnd.ToShortTimeString());
            }
            return calendarEvent.DateStart.ToString();
        }

        private bool eventApproprieteForSend(CalendarEvent calendarEvent)
        {
            if (calendarEvent.IsAllDay) return allDayEventRule(calendarEvent);

            if (calendarEvent.EventType == EventType.Personal) return personalEventRule(calendarEvent);

            if (calendarEvent.EventType == EventType.Meeting) return meetingEventRule(calendarEvent);

            return false;
        }

        private bool allDayEventRule(CalendarEvent calendarEvent)
        {
            return calendarEvent.DateStart.Date <= Config.Instance.Now + _timeBeforeAllDayReminder;
        }

        private bool personalEventRule(CalendarEvent calendarEvent)
        {
            return calendarEvent.DateStart <= Config.Instance.Now + _timeBeforeReminder;
        }

        private bool meetingEventRule(CalendarEvent calendarEvent)
        {
            return calendarEvent.DateStart <= Config.Instance.Now + _timeBeforeReminder;
        }

        public DateTime GetNextTimeForStart()
        {
            return Config.Instance.Now + _repeatPeriod;
        }
    }
}
