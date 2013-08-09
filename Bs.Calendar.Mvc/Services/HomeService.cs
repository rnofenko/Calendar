using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class HomeService
    {
        private readonly UsersRules _rules;

        public HomeService(UsersRules rules)
        {
            _rules = rules;
            var unit = Ioc.Resolve<RepoUnit>();
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
    }
}