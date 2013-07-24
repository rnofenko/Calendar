using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

using Bs.Calendar.Core.Utilities;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public enum BirthdateRange
    {
        DAY,
        WEEK,
        MONTH,
        YEAR
    }

    public class HomeService
    {
        private readonly RepoUnit _repoUnit;

        public HomeService(RepoUnit repository)
        {
            _repoUnit = repository;
        }

        public HomeBirthdayListVm List(BirthdateRange range)
        {
            var birthdays = new HomeBirthdayListVm();

            var today = DateTime.Today;
            var isLeapYear = DateTime.IsLeapYear(today.Year);

            switch(range)
            {
                case BirthdateRange.YEAR:
                    {
                        birthdays.GuysWhoHaveBirthdays = _repoUnit.User
                            .Load(user => 
                                isLeapYear ||
                                !isLeapYear &&
                                user.Birthdate.Month != 2 &&
                                user.Birthdate.Day != 29)
                            .ToList();

                        break;
                    }
                case BirthdateRange.MONTH:
                    {
                        birthdays.GuysWhoHaveBirthdays = _repoUnit.User
                            .Load(user =>
                                (isLeapYear && today.Month == user.Birthdate.Month) ||
                                (!isLeapYear && today.Month == user.Birthdate.Month &&
                                    ((user.Birthdate.Month != 2) || (user.Birthdate.Month == 2 && user.Birthdate.Day != 29))))
                            .ToList();

                        break;
                    }
                case BirthdateRange.WEEK:
                    {
                        DateTime currentWeekStart = today.FirstDayOfWeek();
                        DateTime currentWeekEnd = today.LastDayOfWeek();

                        birthdays.GuysWhoHaveBirthdays = _repoUnit.User
                            .Load(user =>
                                user.Birthdate.Month == currentWeekStart.Month && user.Birthdate.Day >= currentWeekStart.Day ||
                                user.Birthdate.Month == currentWeekEnd.Month && user.Birthdate.Day <= currentWeekEnd.Day)
                            .ToList();

                        break;
                    }
                case BirthdateRange.DAY:
                    {
                        birthdays.GuysWhoHaveBirthdays = _repoUnit.User
                            .Load(user =>
                            today.Month == user.Birthdate.Month &&
                            today.Day == user.Birthdate.Day)
                            .ToList();

                        break;
                    }
            }

            return birthdays;
        }

        public int NewAge(DateTime birthdate)
        {
            DateTime currentDay = DateTime.Now;

            int newAge = currentDay.Year - birthdate.Year;

            return birthdate > currentDay.AddYears(-newAge) ? -- newAge : newAge;
        }

        public HomeBirthdayListVm List()
        {
            var users = _repoUnit.User.Load().ToList();

            if (!users.Any())
            {
                var rRandom = new Random(Environment.TickCount);

                _repoUnit.User.Save(new User
                                        {
                                            Email = "somemail@gmail.com",
                                            Role = Roles.None,
                                            FirstName = "Paul",
                                            LastName = "Porfiroff",
                                            Birthdate = new DateTime(rRandom.Next(1700, 2013), rRandom.Next(1, 12), rRandom.Next(1, 28))
                                        });

                //unit.User.Save(new User {Email = "rnofenko@gmail.com", Role = Roles.Admin, 
                //    FirstName = "Roman", LastName = "Nofenko"});
                users = _repoUnit.User.Load().ToList();
            }

            return new HomeBirthdayListVm() {GuysWhoHaveBirthdays = users};
        }
    }
}