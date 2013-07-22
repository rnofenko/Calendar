using System;
using System.Web.Security;
using Bs.Calendar.DataAccess;

namespace Bs.Calendar.Mvc.Services
{
    class CalendarMembershipProvider : MembershipProvider
    {
        private readonly RepoUnit _unit;

        public CalendarMembershipProvider(RepoUnit unit)
        {
            _unit = unit;
        }

        #region 

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
            throw new NotImplementedException();
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

        #endregion

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

        #region 

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        #region 

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

        #endregion
    

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        #region

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

        #endregion

        public override bool ValidateUser(string userEmail, string password)
        {
            var crypto = new CryptoProvider();
            var user = _unit.User.Get(
                u => u.Email == userEmail &&
                u.PasswordKeccakHash == crypto.GetKeccakHash(password) &&
                u.PasswordSkeinHash == crypto.GetSkeinHash(password));
            return user != null;
        }
    }
}
