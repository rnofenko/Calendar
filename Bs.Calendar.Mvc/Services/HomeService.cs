using System;
using System.Collections.Generic;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class HomeService
    {
        public IEnumerable<User> LoadUsers()
        {
            using (var unit = new RepoUnit())
            {
                var today = DateTime.Now;
                var endOfTheMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                return new UsersRules().LoadUsersByBirthday(today, endOfTheMonth);
            }
        }
    }
}