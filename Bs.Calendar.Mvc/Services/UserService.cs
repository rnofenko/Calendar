using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Security;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;
using Roles = Bs.Calendar.Models.Roles;

namespace Bs.Calendar.Mvc.Services
{
    public class UserService
    {
        private readonly RepoUnit _unit;
        private readonly ContactService _contactService;
        private const int SALT_LENGTH = 128;
        public int PageSize { get; set; }

        public UserService(RepoUnit unit, ContactService contactService)
        {
            PageSize = 7;
            _contactService = contactService;
            _unit = unit;
        }

        public User GetUser(int userId)
        {
            var user = _unit.User.Get(userId);
            return user;
        }

        public User GetCurrentUser()
        {
            var membershipUserEmail = Membership.GetUser().Email;
            var user = _unit.User.Get(u => u.Email == membershipUserEmail);
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _unit.User.Load().ToList();
        }

        public void SaveUser(UserEditVm userModel)
        {
            if (!EmailSender.IsValidEmailAddress(userModel.Email))
            {
                throw new WarningException(string.Format("{0} - is not valid email address", userModel.Email));
            }
            if (_unit.User.Get(u => u.Email == userModel.Email) != null)
            {
                throw new WarningException(string.Format("User with email {0} already exists", userModel.Email));
            }

            var contacts = _contactService.UpdateContacts(userModel.Contacts);

            var user = new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Role = userModel.Role,
                BirthDate = userModel.BirthDate,
                Contacts = contacts,

                Live = userModel.Live,
                ApproveState = userModel.ApproveState
            };
            _unit.User.Save(user);
        }

        public void UpdateUserState(int userModelId, ApproveStates approveState = ApproveStates.NotApproved, LiveStatuses liveState = LiveStatuses.Active)
        {
            var user = _unit.User.Get(userModelId);
            
            user.Live = liveState;
            user.ApproveState = approveState;

            _unit.User.Save(user);
        }

        public void DeleteUser(int id)
        {
            _unit.User.Delete(_unit.User.Get(id));
        }

        public bool CreateUser(UserEditVm userEditVm)
        {
            var dbUser = _unit.User.Get(u => u.Email == userEditVm.Email);

            if (dbUser != null)
            {
                userEditVm.FirstName = dbUser.FirstName;
                userEditVm.LastName = dbUser.LastName;
                userEditVm.Role = dbUser.Role;

                if (dbUser.Live == LiveStatuses.Deleted)
                {
                    userEditVm.Live = dbUser.Live;
                }

                return false;
            }

            userEditVm.ApproveState = ApproveStates.NotApproved;
            userEditVm.Live = LiveStatuses.Active;

            SaveUser(userEditVm);
            dbUser = _unit.User.Get(u => u.Email == userEditVm.Email);
            dbUser.PasswordHash = userEditVm.Email;
            _unit.User.Save(dbUser);

            return true;
        }

        public void EditUser(UserEditVm userModel, bool deleted)
        {
            var userToEdit = GetUser(userModel.UserId);
            if (!EmailSender.IsValidEmailAddress(userModel.Email))
            {
                throw new WarningException(string.Format("{0} - is not valid email address", userModel.Email));
            }
            if (userToEdit.Email != userModel.Email && _unit.User.Get(u => u.Email == userModel.Email) != null)
            {
                throw new WarningException(string.Format("User with email {0} already exists", userModel.Email));
            }
            if (userModel.Email != userToEdit.Email)
            {
                SendMsgToUser(userToEdit);
            }

            userToEdit.FirstName = userModel.FirstName;
            userToEdit.LastName = userModel.LastName;
            userToEdit.Email = userModel.Email;
            userToEdit.Role = userModel.Role;
            userToEdit.Live = deleted ? LiveStatuses.Deleted : userToEdit.Live;
            userToEdit.BirthDate = userModel.BirthDate;

            var contacts = _contactService.UpdateContacts(userModel.Contacts);
            userToEdit.Contacts.Clear();
            userToEdit.Contacts = contacts;

            _unit.User.Save(userToEdit);
        }

        private static void SendMsgToUser(User user)
        {
            var sender = Ioc.Resolve<EmailSender>();
            var body = string.Format("Hi, {0}!\nYour account's status is {1} and {2} now.",
                user.FullName,
                user.Live == LiveStatuses.Active ? "active" : "deleted",
                user.ApproveState == ApproveStates.Approved ? "approved" : "not approved");

            sender.Send("Status has been changed", body, user.Email);
        }

        public UsersVm RetreiveList(PagingVm pagingVm)
        {
            var users = _unit.User.Load();

            users = searchByStr(users, pagingVm.SearchStr);
            users = searchByRoleAndState(users, pagingVm.RolesFilter, pagingVm.LiveStatusFilter, pagingVm.ApproveStateFilter,
                                         pagingVm.ShowUnknownRole, pagingVm.ShowUnknownLiveStatus, pagingVm.ShowUnknownApproveState);

            users = sortByStr(users, pagingVm.SortByStr);

            pagingVm = updatePagingVm(pagingVm, users);

            return new UsersVm
            {
                Users = users.Skip((pagingVm.Page - 1) * PageSize).Take(PageSize).ToList(),
                PagingVm = pagingVm
            };
        }

        private PagingVm updatePagingVm(PagingVm pagingVm, IQueryable<User> users)
        {
            var newPagingVm = new PagingVm(pagingVm);

            newPagingVm.TotalPages = PageCounter.GetTotalPages(users.Count(), PageSize);
            newPagingVm.Page = PageCounter.GetRangedPage(pagingVm.Page, newPagingVm.TotalPages);

            return newPagingVm;
        }

        private IQueryable<User> sortByStr(IQueryable<User> users, string sortByStr)
        {
            users = users.OrderByIf(string.IsNullOrEmpty(sortByStr),
                        user => user.Id);

            users = users.OrderByIf(!string.IsNullOrEmpty(sortByStr) && sortByStr.Equals("Name"),
                        team => team.FullName);
            users = users.OrderByDescIf(!string.IsNullOrEmpty(sortByStr) && sortByStr.Equals("NameDesc"),
                        team => team.FullName);

            users = users.OrderByIf(!string.IsNullOrEmpty(sortByStr) && sortByStr.Equals("E-mail"),
                        team => team.Email);
            users = users.OrderByDescIf(!string.IsNullOrEmpty(sortByStr) && sortByStr.Equals("E-mailDesc"),
                        team => team.FullName);

            return users;
        }

        private IQueryable<User> searchByStr(IQueryable<User> users, string searchStr)
        {
            if (string.IsNullOrEmpty(searchStr))
                return users;

            //Delete extra whitespaces
            searchStr = Regex.Replace(searchStr.Trim(), @"\s+", " ").ToLower();

            var filteredUsers = users.WhereIf(searchStr.Length > 0,
                                    user => user.Email.ToLower().Contains(searchStr));

            filteredUsers = filteredUsers.Concat(searchByName(users, searchStr));

            filteredUsers = filteredUsers.Concat(SearchByRole(users, searchStr));

            return filteredUsers.Distinct();
        }

        private IQueryable<User> searchByName(IQueryable<User> users, string searchStr)
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

        private IQueryable<User> SearchByRole(IQueryable<User> users, string searchStr)
        {
            var filteredUsers = Enumerable.Empty<User>().AsQueryable();
            var searchRoleName = Enum.GetNames(typeof(Roles)).FirstOrDefault(role => role.ToLower().Contains(searchStr));

            if (searchRoleName == null)
            {
                return filteredUsers;
            }

            var searchRole = (Roles)Enum.Parse(typeof (Roles), searchRoleName);
            
            filteredUsers = filteredUsers.Concat(users.Where(user => user.Role == searchRole));
            return filteredUsers;
        }

        private IQueryable<User> searchByRoleAndState(
            IQueryable<User> users, Roles showRoles = (Roles)~0,
            LiveStatuses showStatus = (LiveStatuses)~0, ApproveStates showApproveState = (ApproveStates)~0,
            bool selectUnknownRole = false, bool selectUnknownStatus = false, bool selectUnknownState = false)
        {
            //Select users with unknown parameters only if corresponding flags are set to true (even if filter value is stated apparently)

            return users
                .Where(user => (selectUnknownRole ? (showRoles & user.Role) == user.Role : (showRoles & user.Role) != 0) &&
                               (selectUnknownStatus ? (showStatus & user.Live) == user.Live : (showStatus & user.Live) != 0) &&
                               (selectUnknownState ? (showApproveState & user.ApproveState) == user.ApproveState : (showApproveState & user.ApproveState) != 0));
        }

        private int getTotalPages(int count, int pageSize)
        {
            return (int)Math.Ceiling((decimal)count / pageSize);
        }

        private int getRangedPage(int page, int totalPages)
        {
            return page <= 1 ? 1 : page > totalPages ? totalPages : page;
        }

        public void RecoverUser(string email)
        {
            var userToRecover = _unit.User.Get(u => u.Email == email);

            userToRecover.Live = LiveStatuses.Active;
            userToRecover.ApproveState = ApproveStates.NotApproved;

            _unit.User.Save(userToRecover);
        }

        public void ResetPassword(string email)
        {
            var user = _unit.User.Get(u => u.Email == email);

            var saltProvider = Ioc.Resolve<ISaltProvider>();
            var newPassword = saltProvider.GetSalt(7);
            
            user.PasswordSalt = saltProvider.GetSalt(SALT_LENGTH);
            user.PasswordHash = Ioc.Resolve<ICryptoProvider>().GetHashWithSalt(newPassword, user.PasswordSalt);
            _unit.User.Save(user);

            var sender = Ioc.Resolve<EmailSender>();
            var message = string.Format("Hi {0}! \nYour password has been changed to this new password {1} because of security policy of our web site.", user.FullName, newPassword);
            sender.Send("Password reset", message, email);
        }
    }
}
