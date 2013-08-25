using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Events;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services.Events
{
    public class EventService
    {
        private readonly RepoUnit _unit;
        private readonly UsersRules _rules;
        private readonly EventSavingService _savingService;

        public EventService(RepoUnit repoUnit, EventSavingService eventSavingService, UsersRules rules)
        {
            _unit = repoUnit;
            _rules = rules;
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
    }
}