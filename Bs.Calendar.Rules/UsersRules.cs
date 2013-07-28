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
            return _unit.User.Load(u => u.BirthDate >= from && u.BirthDate <= into);
        }
    }
}
