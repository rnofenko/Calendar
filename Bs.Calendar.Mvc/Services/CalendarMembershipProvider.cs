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
            if (!UserService.IsValidEmailAddress(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }
            if (GetUser(email, true) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            var crypto = new CryptoProvider();
            var user = new User
            {
                FirstName = email.Remove(email.IndexOf('@')),
                LastName = "",
                Email = email,
                PasswordKeccakHash = crypto.GetKeccakHash(password),
                PasswordMd5Hash = crypto.GetMd5Hash(password),
                Role = Roles.None
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
                string.Format("{0} {1}", user.FirstName, user.LastName),
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
                return string.Format("{0} {1}", user.FirstName, user.LastName);
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

        /// <summary>
        /// Validate user.
        /// </summary>
        /// <param name="userEmail">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <returns></returns>
        public override bool ValidateUser(string userEmail, string password)
        {
            var crypto = new CryptoProvider();
            using (var unit = new RepoUnit())
            {
                var keccakHash = crypto.GetKeccakHash(password);
                var md5Hash = crypto.GetMd5Hash(password);
                var user = unit.User.Get(
                    u => (u.Email == userEmail &&
                         u.PasswordKeccakHash == keccakHash &&
                         u.PasswordMd5Hash == md5Hash));
                return user != null;
            }
        }
    }
}
