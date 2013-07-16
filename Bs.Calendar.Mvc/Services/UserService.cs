using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
            return _unit.User.Get(userId);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _unit.User.Load().ToList();
        }

        public void SaveUser(UserEditVm userModel)
        {
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
                userToEdit.FirstName = userToEdit.FirstName;
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
    }
}