using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class HomeService
    {
        private readonly UsersRules _rules;
        private readonly RepoUnit _unit;

        public HomeService(UsersRules rules, RepoUnit unit)
        {
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

        public IEnumerable<User> LoadUsers()
        {
            var today = DateTime.Now;
            return _rules.LoadUsersByBirthday(today, new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)));
        }

        public List<EventVm> GetEvents(DateTime from, DateTime to)
        {
            return getBirthdayEvents(from, to);
        }

        private List<EventVm> getBirthdayEvents(DateTime from, DateTime to)
        {
            var users = _rules.LoadUsersByBirthday(from, to);
            return users.Select(u => new EventVm { Date = u.BirthDate.Value, Text = u.LastName }).ToList();
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