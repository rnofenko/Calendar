using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;
using Microsoft.Ajax.Utilities;

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

        public void EditUser(UserEditVm userModel, bool delete)
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
            userToEdit.LiveState = delete ? LiveState.Deleted : userModel.LiveState;
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

            pagingVm = updatePagingVm(pagingVm, users);

            return new UsersVm
            {
                Users = users.Skip((pagingVm.Page - 1) * PageSize).Take(PageSize).ToList(),
                PagingVm = pagingVm
            };
        }

        private PagingVm updatePagingVm(PagingVm pagingVm, IQueryable<User> users)
        {
            var totalPages = PageCounter.GetTotalPages(users.Count(), PageSize);
            var currentPage = PageCounter.GetRangedPage(pagingVm.Page, totalPages);

            return new PagingVm(pagingVm.SearchStr, pagingVm.SortByStr, totalPages, currentPage);
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

            //filteredUsers = filteredUsers.Concat(SearchByRole(users, searchStr));

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

        private IEnumerable<User> SearchByRole(IQueryable<User> users, string searchStr)
        {
            searchStr = searchStr.Insert(1, searchStr[0].ToString(CultureInfo.InvariantCulture).ToUpper());
            searchStr = searchStr.Remove(0, 1);

            var filteredUsers = Enumerable.Empty<User>().AsQueryable();

            if (string.IsNullOrEmpty(searchStr))
                return users;            

            var searchRole = (Roles)Enum.Parse(typeof(Roles), searchStr);
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
    }
}
