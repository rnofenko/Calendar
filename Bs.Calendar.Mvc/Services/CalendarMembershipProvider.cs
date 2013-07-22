using System;
using System.Web.Security;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.Services
{
    class CalendarMembershipProvider : MembershipProvider
    {
        private readonly RepoUnit _unit;

        public CalendarMembershipProvider(RepoUnit unit)
        {
            _unit = unit;
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != string.Empty)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var user = _unit.User.Get(u => u.Email == email);

            if (user == null)
            {
                var userObj = new User
                    {
                        FirstName = username.Split(' ')[0],
                        LastName = username.Split(' ')[1],
                        Password = GetHash(password),
                        Email = email
                    };

                _unit.User.Save(userObj);
                status = MembershipCreateStatus.Success;

                return GetUser(user.Email, true);
            }
            status = MembershipCreateStatus.DuplicateUserName;

            return null;
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

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
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
            var user = _unit.User.Get(u => u.Email == email);

            if (user != null)
            {
                var memUser = new MembershipUser("CustomMembershipProvider",
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
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            var user = _unit.User.Get(u => u.Email == email);
            return string.Format("{0} {1}", user.FirstName, user.LastName);
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
            var user = _unit.User.Get(u => u.Email == userEmail && u.Password == GetHash(password));
            return user != null;
        }

        private string GetHash(string password)
        {
            throw new NotImplementedException();
        }
    }
}
