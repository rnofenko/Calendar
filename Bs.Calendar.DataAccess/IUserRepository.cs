using System;
using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public interface IUserRepository : IRepository<User>
    {
        IQueryable<User> Load(UserFilter filter);
        event Action<IQueryable<User>> OnBeforePaging;
    }
}
