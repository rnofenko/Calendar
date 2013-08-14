using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Helpers;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.Services
{
    public interface IUserFilterService
    {
        IQueryable<User> ApplyFilter(IQueryable<User> users, UserFilter filter);
        IQueryable<User> ApplyFilter(IQueryable<User> users, string searchString);
        IQueryable<User> ApplyFilter(IQueryable<User> users, Roles showRoles, LiveStatuses showStatus, ApproveStates showApproveState);
    }
}