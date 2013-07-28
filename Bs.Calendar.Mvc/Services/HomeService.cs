using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public IEnumerable<User> LoadUsers()
        {
            var today = DateTime.Now;
            var endOfTheMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            return _rules.LoadUsersByBirthday(today, endOfTheMonth).ToList();
        }
    }
}