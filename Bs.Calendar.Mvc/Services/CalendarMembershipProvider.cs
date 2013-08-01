using System;
using System.Web.Security;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;
using Roles = Bs.Calendar.Models.Roles;

namespace Bs.Calendar.Mvc.Services
{
    public class CalendarMembershipProvider : MembershipProvider
    {
        private readonly ICryptoProvider _crypto;
        private readonly ISaltProvider  _salt;
        private const  int SALT_LENGTH = 128;

        public CalendarMembershipProvider(ICryptoProvider crypto, ISaltProvider salt)
        {
            _crypto = crypto;
            _salt = salt;
        }

        public CalendarMembershipProvider(){}

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string passwordQuestion, string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {
            if(!EmailSender.IsValidEmailAddress(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            if (GetUser(email, true) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return GetUser(email, false);                
            }
            var salt = _salt.GetSalt(SALT_LENGTH);
            var user = new User
            {
                FirstName = email.Remove(email.IndexOf('@')),
                LastName = "",
                Email = email,
                PasswordHash = _crypto.GetHashWithSalt(password, salt),
                PasswordSalt = salt,
                Role = Roles.None,
                LiveState = LiveState.NotApproved,
                BirthDate = null
            };           
            using (var unit = new RepoUnit())
            {
                unit.User.Save(user);
                status = MembershipCreateStatus.Success;
                return GetUser(email, true);
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string email, bool userIsOnline)
        {
            User user;

            using (var unit = new RepoUnit())
            {
                user = unit.User.Get(u => u.Email == email);
            }

            if (user == null) return null;
            var memUser = new MembershipUser("CalendarMembershipProvider",
                user.FullName,
                user.Id, user.Email,
                string.Empty,
                string.Empty,
                true,
                false,
                DateTime.Now,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.Now,
                DateTime.Now);
            return memUser;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            using (var unit = new RepoUnit())
            {
                var user = unit.User.Get(u => u.Email == email);
                return user.FullName;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string userEmail, string password)
        {
            using (var unit = new RepoUnit())
            {
                var user = unit.User.Get(u => u.Email == userEmail);
                if (user == null) return false;

                var passwordSalt = user.PasswordSalt;
                var passwordHash = _crypto.GetHashWithSalt(password, passwordSalt);

                if (user.PasswordHash == passwordHash) return true;
            }
            return false;
        }
    }
}
