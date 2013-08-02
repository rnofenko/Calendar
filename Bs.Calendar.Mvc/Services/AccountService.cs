using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using System.Web.Security;
using Bs.Calendar.Rules;
using Bs.Calendar.Core;
using Roles = Bs.Calendar.Models.Roles;

namespace Bs.Calendar.Mvc.Services
{
    public class AccountService
    {
        private readonly RepoUnit _unit;
        private readonly CalendarMembershipProvider _membershipProvider;
        private const int SALT_LENGTH = 128;

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
            passwordRecovery.PasswordSalt = Resolver.Resolve<ISaltProvider>().GetSalt(SALT_LENGTH);
            passwordRecovery.PasswordHash = Resolver.Resolve<ICryptoProvider>().GetHashWithSalt(DateTime.Now.ToString(CultureInfo.InvariantCulture), passwordRecovery.PasswordSalt);
            _unit.User.Save(user);
            
            var sender = Resolver.Resolve<EmailSender>();
            var message = string.Format("{0}/PasswordReset/{1}/{2}", url.Remove(url.LastIndexOf('/')), user.Id, passwordRecovery.PasswordHash);
            sender.SendEmail(new MailMessage("info@binary-studio.com", email, "Password Recovery", message));
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

            user.PasswordSalt = Resolver.Resolve<ISaltProvider>().GetSalt(SALT_LENGTH);
            user.PasswordHash = Resolver.Resolve<ICryptoProvider>().GetHashWithSalt(model.Password, user.PasswordSalt);
            user.PasswordRecovery.PasswordHash = "";
            user.PasswordRecovery.PasswordSalt = "";

            _unit.User.Save(user);
        }

        public ContactType GetContactType(string contact)
        {
            if (string.IsNullOrEmpty(contact)) return ContactType.None;

            if (contact.StartsWith("@")) return ContactType.Twitter;
            
            if (EmailSender.IsValidEmailAddress(contact)) return ContactType.Email;

            if (contact.All(char.IsNumber)) return ContactType.Phone;

            if (Uri.IsWellFormedUriString(contact, UriKind.Absolute)) return ContactType.UrlAdress;

            return ContactType.Skype;
        }
    }
}
