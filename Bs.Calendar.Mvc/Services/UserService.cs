using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public class UserService
    {
        private readonly RepoUnit _unit;

        public UserService(RepoUnit unit)
        {
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
                throw new WarningException("{0} - is not valid email address", userModel.Email);
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
                throw new WarningException("{0} - is not valid email address", userModel.Email);
            }
            if(userToEdit.Email != userModel.Email && _unit.User.Get(u => u.Email == userModel.Email) != null)
            {
                throw new WarningException(string.Format("User with email {0} already exists", userModel.Email));
            }            
            userToEdit.FirstName = userModel.FirstName;
            userToEdit.LastName = userModel.LastName;
            userToEdit.Email = userModel.Email;
            userToEdit.Role = userModel.Role;
            _unit.User.Save(userToEdit);
        }

        public bool IsValidEmailAddress(string emailaddress)
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

        public UsersVm Find(string searchStr)
        {
            var users = _unit.User.Load().AsEnumerable();

            if (string.IsNullOrEmpty(searchStr))
            {
                return new UsersVm {Users = users.ToList()};
            }

            //Delete extra whitespaces
            searchStr = Regex.Replace(searchStr.Trim(), @"\s+", " ");

            if (searchStr.Contains('@') && IsValidEmailAddress(searchStr))
            {
                users = users.Where(user => user.Email.Equals(
                              searchStr, StringComparison.InvariantCulture));
            } 
            else if (searchStr.Length != 0)
            {
                users = FindByName(users, searchStr);
            }

            return new UsersVm {Users = users.ToList()};
        }

        private IEnumerable<User> FindByName(IEnumerable<User> users, string searchStr)
        {
            var arrName = searchStr.Split();
            var comparisonType = StringComparison.InvariantCultureIgnoreCase;

            var filteredUsers = users.Where(user =>
                user.FirstName.Equals(arrName[0], comparisonType) ||
                user.LastName.Equals(arrName[0], comparisonType));

            if (arrName.Length == 2)
            {
                filteredUsers = filteredUsers.Where(user =>
                    user.FirstName.Equals(arrName[1], comparisonType) ||
                    user.LastName.Equals(arrName[1], comparisonType));
            }

            return filteredUsers;
        }
    }
}