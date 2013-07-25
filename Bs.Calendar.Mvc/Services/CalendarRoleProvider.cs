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
            return (Enum.GetValues(typeof(Models.Roles))
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
            using (var unit = new RepoUnit())
            {
                var usersInRole = unit.User.Get(u => u.Role.ToString() == roleName);
                if (usersInRole == null) return null;
                var usersInRoleEmails = usersInRole.Email;
                return new[] {usersInRoleEmails};
            }
        }

        public override string[] GetAllRoles()
        {
            return Enum.GetNames(typeof(Models.Roles));
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}