using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

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
    }
}