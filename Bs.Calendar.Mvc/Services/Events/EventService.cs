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
            var currentUserId = _unit.User.Load(user => user.Email == currentUserEmail).First().Id;
            _savingService.Save(calendarEventVm, currentUserId);
        }

        public IEnumerable<TeamVm> GetTeams() 
        {
            var teams = _unit.Team.Load(t => t.Live == LiveStatuses.Active).ToList();
            return teams.Select(t => new TeamVm(t)).ToList();
        }

        public IEnumerable<UserVm> GetAllUsers() 
        {
            var users = _unit.User.Load(u => u.Live == LiveStatuses.Active).ToList();
            return users.Select(u => new UserVm(u)).ToList();
        }

        public List<RoomEventVm> GetRooms(DateTime date)
        {
            var rooms = new List<RoomEventVm>();
            _unit.Room.Load().OrderBy(r => r.Name).ToList().ForEach(r => rooms.Add(new RoomEventVm(r)));

            var groupedEvents = _unit.PersonalEvent.Load(p => p.Event.EventType == EventType.Meeting && p.Event.DateStart.Date == date)
                                .Select(p => p.Event)
                                .Union(_unit.TeamEvent.Load(t => t.Event.EventType == EventType.Meeting && t.Event.DateStart.Date == date)
                                .Select(t => t.Event))
                                .GroupBy(e => e.Room).ToList();

            foreach (var room in rooms)
            {
                var group = groupedEvents.Find(g => g.Key == room.Room);
                room.TimePeriod = group.Select(e => new Tuple<DateTime, DateTime>(e.DateStart, e.DateEnd)).ToList();
            }

            return rooms;
        }
    }
}