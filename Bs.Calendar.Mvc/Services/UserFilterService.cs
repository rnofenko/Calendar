using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using System.Linq.Expressions;

namespace Bs.Calendar.Mvc.Services
{
    public class UserFilterService : IUserFilterService
    {
        public IQueryable<User> ApplyFilter(IQueryable<User> users, Roles showRoles, LiveStatuses showStatus, ApproveStates showApproveState)
        {
            return users
                .WhereIf(showRoles > 0 && showRoles < Roles.All, user => (user.Role & showRoles) > 0)
                .WhereIf(showStatus > 0 && showStatus < LiveStatuses.All, user => (user.Live & showStatus) > 0)
                .WhereIf(showApproveState > 0 && showApproveState < ApproveStates.All, user => (user.ApproveState & showApproveState) > 0);
        }

        public IQueryable<User> ApplyFilter(IQueryable<User> users, UserFilter filter)
        {
            return
                Enumerable.Empty<User>().AsQueryable()
                    .Concat(ApplyFilter(users, filter.SearchString))
                    .Concat(ApplyFilter(users, filter.RoleFilter, filter.LiveStatusFilter, filter.ApproveStateFilter))
                    .OrderByExpression(filter.SortBy);
        }

        public IQueryable<User> ApplyFilter(IQueryable<User> users, string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return users;

            //Delete extra whitespaces
            searchString = Regex.Replace(searchString.Trim(), @"\s+", " ").ToLower();
            
            return users
                .WhereIf(searchString.Length > 0, user => user.Email.ToLower().Contains(searchString))
                .Concat(filterByName(users, searchString))
                .Concat(filterByRole(users, searchString))
                .Distinct();
        }

        private IQueryable<User> filterByRole(IQueryable<User> users, string stringRole)
        {
            Roles role;
            return users.WhereIf(Enum.TryParse(stringRole, true, out role), user => user.Role == role);
        }

        private IQueryable<User> filterByName(IQueryable<User> users, string searchStr)
        {
            var filteredUsers = Enumerable.Empty<User>().AsQueryable();
            var splitedStr = searchStr.Split();

            for (int i = 0; i < splitedStr.Rank && i < 2; i++)
            {
                var str = splitedStr[i];
                filteredUsers = filteredUsers.Concat(users.Where(user => user.FullName.ToLower().Contains(str)));
            }

            return filteredUsers;
        }
    }
}