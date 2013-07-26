using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

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
            if (!IsValidEmailAddress(userModel.Email)) 
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
                Role = userModel.Role
                LiveState = userModel.LiveState
            };
            _unit.User.Save(user);
        }

        public void DeleteUser(int id)
        {
            _unit.User.Delete(_unit.User.Get(id));
        }

        public void EditUser(UserEditVm userModel) 
        {
            var userToEdit = GetUser(userModel.UserId);
            if (!IsValidEmailAddress(userModel.Email)) 
            {
                throw new WarningException(string.Format("{0} - is not valid email address", userModel.Email));
            }
            if (userToEdit.Email != userModel.Email && _unit.User.Get(u => u.Email == userModel.Email) != null) 
            {
                throw new WarningException(string.Format("User with email {0} already exists", userModel.Email));
            }
            userToEdit.FirstName = userModel.FirstName;
            userToEdit.LastName = userModel.LastName;
            userToEdit.Email = userModel.Email;
            userToEdit.Role = userModel.Role;
            _unit.User.Save(userToEdit);
        }

        public static bool IsValidEmailAddress(string emailaddress) 
        {
            try 
            {
                var email = new MailAddress(emailaddress);
                return true;
            } 
            catch (FormatException) 
            {
                return false;
            }
        }

        public UsersVm RetreiveList(PagingVm pagingVm)
        {
            var users = _unit.User.Load();

            if (!string.IsNullOrEmpty(pagingVm.SearchStr)) 
            {
                users = find(users, pagingVm.SearchStr);
            }

            if (!string.IsNullOrEmpty(pagingVm.SortByStr))
            {
                users = sort(users, pagingVm.SortByStr);
            }
            else
            {
                users = users.OrderBy(user => user.Id);
            }

            var totalPages = (int)Math.Ceiling((decimal)users.Count() / PageSize);
            var currentPage = pagingVm.Page <= 1 ? 1 : pagingVm.Page > totalPages ? totalPages : pagingVm.Page;

            return new UsersVm 
            {
                Users = users.Skip((currentPage - 1) * PageSize).Take(PageSize).ToList(),
                PagingVm = new PagingVm(pagingVm.SearchStr, pagingVm.SortByStr, totalPages, currentPage)
            };
        }

        private IQueryable<User> sort(IQueryable<User> users, string sortByStr) 
        {
            switch (sortByStr) 
            {
                case "Name":
                    users = users.OrderBy(user => user.FirstName).ThenBy(user => user.LastName);
                    break;
                case "E-mail":
                    users = users.OrderBy(user => user.Email);
                    break;
            }
            return users;
        }

        private IQueryable<User> find(IQueryable<User> users, string searchStr) 
        {
            //Delete extra whitespaces
            searchStr = Regex.Replace(searchStr.Trim(), @"\s+", " ");

            if (searchStr.Contains('@') && IsValidEmailAddress(searchStr))
            {
                users = users.Where(user => user.Email.Equals(
                              searchStr, StringComparison.InvariantCulture));
            } 
            else if (searchStr.Length != 0) 
            {
                users = findByName(users, searchStr);
            }

            return users;
        }

        private IQueryable<User> findByName(IQueryable<User> users, string searchStr) 
        {
            var arrName = searchStr.Split();
            
            var comparisonType = StringComparison.InvariantCultureIgnoreCase;
            var firstName = arrName[0];
           
            var filteredUsers = users.Where(user =>
                user.FirstName.Equals(firstName, comparisonType) ||
                user.LastName.Equals(firstName, comparisonType));

            if (arrName.Length == 2)
            {
                var lastName = arrName[1];
                filteredUsers = filteredUsers.Where(user =>
                    user.FirstName.Equals(lastName, comparisonType) ||
                    user.LastName.Equals(lastName, comparisonType));
            }

            return filteredUsers;
        }
    }
}
