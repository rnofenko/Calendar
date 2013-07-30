using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using System.Web.Security;
using Bs.Calendar.Rules;
using Roles = Bs.Calendar.Models.Roles;

namespace Bs.Calendar.Mvc.Services
{
    public class AccountService
    {
        private readonly RepoUnit _unit;
        private readonly CalendarMembershipProvider _membershipProvider;

        public AccountService(RepoUnit unit, CalendarMembershipProvider provider)
        {
            _unit = unit;
            _membershipProvider = provider;
        }

        public bool LoginUser(AccountVm model)
        {
            if (!_membershipProvider.ValidateUser(model.Email, model.Password)) return false;
            var user = _unit.User.Get(u => u.Email == model.Email);
            user.LiveState = user.LiveState == LiveState.Deleted ? LiveState.NotApproved : user.LiveState;
            _unit.User.Save(user);
            FormsAuthentication.SetAuthCookie(model.Email, true);
            return true;
        }

        public bool? RegisterUser(AccountVm account, out MembershipCreateStatus status)
        {
            _membershipProvider.CreateUser("", account.Password, account.Email, "", "", true, null, out status);
            if (status == MembershipCreateStatus.Success)
            {
                SendMsgToAdmins(account.Email);
                FormsAuthentication.SetAuthCookie(account.Email, false);
                return true;
            }
            if (status == MembershipCreateStatus.DuplicateEmail)
            {
                var user = _unit.User.Get(u => u.Email == account.Email);
                if (user.LiveState == LiveState.Deleted)
                {
                    return false;
                }
            }
            return null;
        }

        private static void SendMsgToAdmins(string emailAddress)
        {
            var sender = new EmailSender();
            const string subject = "New user registration";
            var body = string.Format("Hi there!\nA new user with email {0} has been added to the calendar!",
                emailAddress);
            using (var unit = new RepoUnit())
            {
                foreach (var admin in unit.User.Load(u => u.Role == Roles.Admin).ToList())
                {
                    var msg = new MailMessage(emailAddress, admin.Email)
                    {
                        Subject = subject,
                        Body = body
                    };
                    sender.SendEmail(msg);
                }
            }
        }

        
        public User GetUser(int userId)
        {
            var user = _unit.User.Get(userId);
            return user;
        }

        public UserEditVm GetUserEditVm(string email)
        {
            var user = _unit.User.Load(u => u.Email.Equals(email)).First();
            return new UserEditVm(user);
        }

        public void EditUser(UserEditVm userEditVm)
        {
            var userToEdit = GetUser(userEditVm.UserId);

            if (!IsValidEmailAddress(userEditVm.Email)) 
            {
                throw new WarningException(string.Format("{0} - is not valid email address", userEditVm.Email));
            }
            if (userToEdit.Email != userEditVm.Email && _unit.User.Get(u => u.Email == userEditVm.Email) != null) 
            {
                throw new WarningException(string.Format("User with email {0} already exists", userEditVm.Email));
            }

            userToEdit.FirstName = userEditVm.FirstName;
            userToEdit.LastName = userEditVm.LastName;
            userToEdit.Email = userEditVm.Email;
            userToEdit.BirthDate = userEditVm.BirthDate;

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
        
    }
}