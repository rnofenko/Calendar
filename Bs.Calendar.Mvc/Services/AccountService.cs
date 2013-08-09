using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using System.Web.Security;
using Bs.Calendar.Rules;
using Bs.Calendar.Core;
using Bs.Calendar.Rules.Emails;
using Roles = Bs.Calendar.Models.Roles;

namespace Bs.Calendar.Mvc.Services
{
    public class AccountService
    {
        private readonly RepoUnit _unit;
        private readonly ContactService _contactService;
        private readonly CalendarMembershipProvider _membershipProvider;
        private const int SALT_LENGTH = 128;

        public AccountService(RepoUnit unit, ContactService contactService, CalendarMembershipProvider provider)
        {
            _unit = unit;
            _contactService = contactService;
            _membershipProvider = provider;
        }

        public bool LoginUser(AccountVm model)
        {
            if (!_membershipProvider.ValidateUser(model.Email, model.Password)) return false;
            var user = _unit.User.Get(u => u.Email == model.Email);

            user.ApproveState = user.Live == LiveStatuses.Deleted ? ApproveStates.NotApproved : user.ApproveState;

            _unit.User.Save(user);
            FormsAuthentication.SetAuthCookie(model.Email, true);
            return true;
        }

        public bool? RegisterUser(RegisterVm account, out MembershipCreateStatus status)
        {
            _membershipProvider.CreateUser("", account.Password, account.Email, "", "", true, null, out status);

            if (status == MembershipCreateStatus.Success)
            {
                //sendMsgToAdmins(account.Email);
                FormsAuthentication.SetAuthCookie(account.Email, false);
                return true;
            }

            if (status == MembershipCreateStatus.DuplicateEmail)
            {
                var user = _unit.User.Get(u => u.Email == account.Email);

                if (user.Live == LiveStatuses.Deleted)
                {
                    return false;
                }
            }

            return null;
        }

        private void sendMsgToAdmins(string emailAddress)
        {
            var sender = Ioc.Resolve<EmailSender>();
            const string SUBJECT = "New user registration";
            var body = string.Format("Hi there!\nA new user with email {0} has been added to the calendar!",
                                     emailAddress);

            sender.Send(SUBJECT, body, _unit.User.Load(u => u.Role == Roles.Admin).Select(x => x.Email));
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

            if (!EmailSender.IsValidEmailAddress(userEditVm.Email))
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

            var contacts = _contactService.UpdateContacts(userEditVm.Contacts);
            userToEdit.Contacts.Clear();
            userToEdit.Contacts = contacts;  

            _unit.User.Save(userToEdit);
        }

        public void PasswordRecovery(string email, string url)
        {
            var user = _unit.User.Load(u => u.Email == email).FirstOrDefault();

            if (user == null)
            {
                throw new WarningException("Can't find that email");
            }

            var passwordRecovery = user.PasswordRecovery ?? (user.PasswordRecovery = new PasswordRecovery());
            passwordRecovery.Date = DateTime.Now;
            passwordRecovery.PasswordSalt = Ioc.Resolve<ISaltProvider>().GetSalt(SALT_LENGTH);
            passwordRecovery.PasswordHash = Ioc.Resolve<ICryptoProvider>().GetHashWithSalt(Guid.NewGuid().ToString(), passwordRecovery.PasswordSalt);
            _unit.User.Save(user);

            var sender = Ioc.Resolve<EmailSender>();
            var message = string.Format("{0}/PasswordReset/{1}/{2}", url.Remove(url.LastIndexOf('/')), user.Id, passwordRecovery.PasswordHash);
            sender.Send("Password Recovery", message, email);
        }

        public AccountVm CheckToken(int id, string token)
        {
            var user = _unit.User.Load(u => u.Id == id && u.PasswordRecovery.PasswordHash == token).FirstOrDefault();

            var expiredDateTime = DateTime.Now - new TimeSpan(24, 0, 0);
            if (user == null || user.PasswordRecovery.Date < expiredDateTime)
            {
                throw new WarningException("Invalid token");
            }

            return new AccountVm { Email = user.Email };
        }

        public void ResetPassword(AccountVm model)
        {
            var user = _unit.User.Load(u => u.Email == model.Email).FirstOrDefault();

            if (user == null)
            {
                throw new WarningException("Something went wrong, try again");
            }

            user.PasswordSalt = Ioc.Resolve<ISaltProvider>().GetSalt(SALT_LENGTH);
            user.PasswordHash = Ioc.Resolve<ICryptoProvider>().GetHashWithSalt(model.Password, user.PasswordSalt);
            user.PasswordRecovery.PasswordHash = "";
            user.PasswordRecovery.PasswordSalt = "";

            _unit.User.Save(user);
        }
    }
}
