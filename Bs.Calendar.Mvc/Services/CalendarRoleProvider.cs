using System;
using System.Linq;
using System.Web.Security;
using Bs.Calendar.DataAccess;

namespace Bs.Calendar.Mvc.Services
{
    public class CalendarRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string email, string roleName)
        {
            using (var unit = new RepoUnit())
            {
                return unit.User.Get(u => u.Email == email && u.Role.ToString() == roleName) != null;
            }
        }

        public override string[] GetRolesForUser(string email)
        {
            using (var unit = new RepoUnit())
            {
                return new[] { unit.User.Get(u => u.Email == email).Role.ToString() };
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            return (Enum.GetValues(typeof(Roles))
                           .Cast<object>().Count(role => roleName == role.ToString())) != 0;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}