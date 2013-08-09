using System;
using System.Collections.Generic;
using System.Linq;
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
            var fromNormalized = NormalizeDate(from);
            var intoNormalized = NormalizeDate(into);

            if (fromNormalized <= intoNormalized)
            {
                var returnedUsers =
                    _unit.User.Load(
                        u =>
                            u.BirthDate != null &&
                            u.Live != LiveStatuses.Deleted &&
                            ((DateTime) u.BirthDate).Month*100 + ((DateTime) u.BirthDate).Day >= fromNormalized &&
                            ((DateTime) u.BirthDate).Month*100 + ((DateTime) u.BirthDate).Day <= intoNormalized);
                            //NormalizeDate((DateTime) u.BirthDate) >= fromNormalized &&
                            //NormalizeDate((DateTime) u.BirthDate) <= intoNormalized);
                return returnedUsers;
            }
            if (fromNormalized > intoNormalized)
            {
                var u1 = _unit.User.Load(u =>
                            u.BirthDate != null &&
                            u.Live != LiveStatuses.Deleted &&
                            NormalizeDate((DateTime) u.BirthDate) >= fromNormalized &&
                            NormalizeDate((DateTime) u.BirthDate) <= 1231);
                var u2 = _unit.User.Load(u =>
                            u.BirthDate != null &&
                            u.Live != LiveStatuses.Deleted &&
                            NormalizeDate((DateTime) u.BirthDate) >= 101 &&
                            NormalizeDate((DateTime) u.BirthDate) <= intoNormalized);
                return u1.Concat(u2);
            }
            return null;
        }

        public int NormalizeDate(DateTime date)
        {
            return date.Month * 100 + date.Day;
        }
    }
}