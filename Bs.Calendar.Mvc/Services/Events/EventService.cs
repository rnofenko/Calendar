using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels.Events;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services.Events
{
    public class EventService
    {
        private readonly RepoUnit _unit;
        private readonly EventSavingService _savingService;

        public EventService(RepoUnit repoUnit, EventSavingService eventSavingService)
        {
            _unit = repoUnit;
            _savingService = eventSavingService;
        }

        public void Save(CalendarEventVm calendarEventVm, string currentUserEmail)
        {
            var currentUser = _unit.User.Get(user => user.Email == currentUserEmail);

            if (currentUser != null)
            {
                _savingService.Save(calendarEventVm, currentUser.Id);
            }
        }

        public int Update(CalendarEventVm calendarEventVm)
        {
            if (calendarEventVm.Id > 0)
            {
                return _savingService.Update(calendarEventVm);
            }

            return 0;
        }

        public IEnumerable<TeamVm> GetTeams() 
        {
            var teams = _unit.Team.Load(t => t.Live == LiveStatuses.Active).ToList();
            return teams.Select(t => new TeamVm(t)).ToList();
        }

        public IEnumerable<UserVm> GetAllUsers() 
        {
            var users = _unit.User.Load(u => u.Live == LiveStatuses.Active && u.ApproveState == ApproveStates.Approved).ToList();
            return users.Select(u => new UserVm(u)).ToList();
        }

        public List<RoomEventVm> GetRooms(DateTime from, DateTime to)
        {
            var rooms = new List<RoomEventVm>();
            _unit.Room
                .Load()
                .OrderBy(r => r.Name)
                .ToList()
                .ForEach(r => rooms.Add(new RoomEventVm(r)));

            var eventRoomGroup = _unit.PersonalEvent.Load(p => p.Event.EventType == EventType.Meeting)
                                .Where(p => p.Event.DateStart >= from && p.Event.DateStart < to)
                                .Select(p => p.Event)
                                .Union(_unit.TeamEvent.Load(t => t.Event.EventType == EventType.Meeting)
                                        .Where(p => p.Event.DateStart >= from && p.Event.DateStart < to)
                                        .Select(t => t.Event))
                                        .GroupBy(e => e.Room).ToList();

            foreach (var room in rooms)
            {
                var group = eventRoomGroup.Find(g => g.Key == room.Room);
                if (group == null) continue;

                room.Events = group.ToList();
            }

            return rooms;
        }

        public List<RoomEventVm> GetRooms(DateTime date)
        {
            return GetRooms(date, date + new TimeSpan(24, 0, 0));
        }

        public CalendarEvent GetEvent(int id)
        {
            var personalEventLink = _unit.PersonalEvent.Get(link => link.Event.Id == id);
            var teamEventLink = _unit.TeamEvent.Get(link => link.Event.Id == id);

            var calendarEvent = personalEventLink == null ? null : personalEventLink.Event;
            calendarEvent = teamEventLink == null ? calendarEvent : teamEventLink.Event;

            return calendarEvent;
        }
    }
}