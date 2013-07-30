using System;
using System.Collections.Generic;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Rules
{
    public class UsersRules
    {
        private readonly RepoUnit _unit;

        public UsersRules(RepoUnit unit)
        {
            _unit = unit;
        }

        public IEnumerable<User> LoadUsersByBirthday(DateTime from, DateTime into)
        {
            return _unit.User.Load(u => u.LiveState != LiveState.Deleted && u.BirthDate >= from && u.BirthDate <= into);
        }

        public IEnumerable<User> LoadUsersByBirthday(DateTime from)
        {
            var days = DateTime.DaysInMonth(from.Year, from.Month);

            return _unit.User.Load(u => u.LiveState != LiveState.Deleted 
                    && ((DateTime)(u.BirthDate)).Month == from.Month
                    && ((DateTime)(u.BirthDate)).Day >= from.Day
                    && ((DateTime)(u.BirthDate)).Day <= days);
        }
    }
}
