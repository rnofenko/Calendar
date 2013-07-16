using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public class UserService
    {
        public User GetUser(int userId)
        {
            using (var unit = new RepoUnit())
            {
                return unit.User.Load().FirstOrDefault(u => u.Id == userId);
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            using (var unit = new RepoUnit())
            {
                return unit.User.Load().ToList();
            }
        }

        public void SaveUser(string firstName, string lastName, string email, Roles role)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role
            };
            using (var unit = new RepoUnit())
            {                
                unit.User.Save(user);
            }            
        }

        public void DeleteUser(int id)
        {
            using (var unit = new RepoUnit())
            {
                unit.User.Delete(GetUser(id));
            }
        }

        public void EditUser(string firstName, string lastName, string email, Roles role, int id)
        {
            using (var unit = new RepoUnit())
            {
                var userToEdit = GetUser(id);
                userToEdit.FirstName = firstName;
                userToEdit.LastName = lastName;
                userToEdit.Email = email;
                unit.User.Save(userToEdit);
            }
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
            //Delete extra whitespaces
            searchStr = Regex.Replace(searchStr.Trim(), @"\s+", " ");
            
            var users = GetAllUsers();
            if (searchStr.Contains('@') && IsValidEmailAddress(searchStr))
            {
                users = FindByEmail(users, searchStr);
            } 
            else if (searchStr.Length != 0)
            {
                users = FindByName(users, searchStr);
            }

            return new UsersVm { Users = users.ToList() };
        }

        private IEnumerable<User> FindByName(IEnumerable<User> users, string searchStr)
        {
            var arrName = searchStr.Split();

            var filteredUsers = users.Where(
                user => user.FirstName.Equals(arrName[0], StringComparison.InvariantCulture));

            if (arrName.Length == 2)
                filteredUsers = filteredUsers.Where(
                    user => user.LastName.Equals(arrName[1], StringComparison.InvariantCulture));

            return filteredUsers;
        } 

        private IEnumerable<User> FindByEmail(IEnumerable<User> users, string searchStr)
        {
            return users.Where(user => user.Email.Equals(
                searchStr, StringComparison.InvariantCulture));
        }   
    }
}