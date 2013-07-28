using System;
using System.Collections.Generic;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Rules
{
    public class UsersRules
    {
        public IEnumerable<User> LoadUsersByBirthday(DateTime from, DateTime into)
        {
            using (var unit = new RepoUnit())
            {
                return unit.User.Load(u => u.BirthDate >= from && u.BirthDate <= into);
            }
        }
    }
}
