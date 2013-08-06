using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;

namespace Bs.Calendar.Mvc.Services
{
    public class UserService
    {
        private readonly RepoUnit _unit;
        public int PageSize { get; set; }

        public UserService(RepoUnit unit)
        {
            PageSize = 7;
            _unit = unit;
        }

        public User GetUser(int userId)
        {
            var user = _unit.User.Get(userId);
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

            var user = new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Role = userModel.Role,
                LiveState = userModel.LiveState,
                BirthDate = userModel.BirthDate
            };
            _unit.User.Save(user);
        }

        public void UpdateUserState(int userModelId, LiveState liveState)
        {
            var user = _unit.User.Get(userModelId);
            user.LiveState = liveState;
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

                if (dbUser.LiveState == LiveState.Deleted)
                {
                    userEditVm.LiveState = dbUser.LiveState;
                }
                return false;
            }
            userEditVm.LiveState = LiveState.NotApproved;
            SaveUser(userEditVm);
            dbUser = _unit.User.Get(u => u.Email == userEditVm.Email);
            dbUser.PasswordHash = userEditVm.Email;
            _unit.User.Save(dbUser);
            return true;
        }

        public void EditUser(UserEditVm userModel, bool active)
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
            userToEdit.LiveState = active
                ? LiveState.Active
                : userToEdit.LiveState == LiveState.NotApproved ? LiveState.NotApproved : LiveState.Deleted;
            userToEdit.BirthDate = userModel.BirthDate;

            _unit.User.Save(userToEdit);
        }

        private static void SendMsgToUser(User user)
        {
            var sender = Ioc.Resolve<EmailSender>();
            var body = string.Format("Hi {0}!\nYour account's status is {1} now.",
                user.FullName, user.LiveState);
            sender.Send("Status has been changed", body, user.Email);
        }

        public UsersVm RetreiveList(PagingVm pagingVm)
        {
            var users = _unit.User.Load();

            users = searchByStr(users, pagingVm.SearchStr);

            users = sortByStr(users, pagingVm.SortByStr);

            var totalPages = PageCounter.GetTotalPages(users.Count(), PageSize);
            var currentPage = PageCounter.GetRangedPage(pagingVm.Page, totalPages);

            return new UsersVm
            {
                Users = users.Skip((currentPage - 1) * PageSize).Take(PageSize).ToList(),
                PagingVm = new PagingVm(pagingVm.SearchStr, pagingVm.SortByStr, totalPages, currentPage)
            };
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
            userToRecover.LiveState = LiveState.NotApproved;
            _unit.User.Save(userToRecover);
        }
    }
}
