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
                var user = unit.User.Get(u => u.Email == email);

                return user != null && user.Role.ToString().Equals(roleName);
            }
        }

        public override string[] GetRolesForUser(string email)
        {
            using (var unit = new RepoUnit())
            {
                var user = unit.User.Get(u => u.Email == email);
                var role = user == null ? Models.Roles.Simple : user.Role;

                return new[] { role.ToString() };
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
            //Can't use Enum.TryParse(), because it allows specifying enum member by value

            return Enum.GetNames(typeof (Models.Roles))
                       .FirstOrDefault(role => role == roleName) != null;
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
            bool roleFound = RoleExists(roleName);

            if (!roleFound)
            {
                throw new NotImplementedException();
            }

            Models.Roles searchedRole;
            Enum.TryParse(roleName, out searchedRole);

            using (var unit = new RepoUnit())
            {
                var usersInRole = unit.User.Get(u => u.Role == searchedRole);

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