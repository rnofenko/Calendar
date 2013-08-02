using System;
using System.Collections.Generic;
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
            var unit = Resolver.Resolve<RepoUnit>();
            if (unit.User.Load() == null)
            {
                var defaultUser = new User
                {
                    BirthDate = DateTime.Now,
                    Email = "admin@gmail.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    LiveState = LiveState.Active,
                    Role = Roles.Admin,
                    PasswordHash = "Admin",
                    PasswordSalt = ""
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