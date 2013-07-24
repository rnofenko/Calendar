using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    public class CalendarMembershipProviderTest
    {
        private CalendarMembershipProvider _membershipProvider;        
        private RepoUnit _unit;
        private User _user;
        private string _userPassword;

        [TestFixtureSetUp]
        public void SetUp()
        {
            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Resolver.Resolve<RepoUnit>();

            _membershipProvider = new CalendarMembershipProvider();

            var crypto = new CryptoProvider();
            _userPassword = "moriarty";
            var keccak = crypto.GetKeccakHash(_userPassword);
            var md5 = crypto.GetMd5Hash(_userPassword);

            _user = new User
            {
                Email = "holmes@email.com",
                FirstName = "Sherlock", LastName = "Holmes",
                PasswordKeccakHash = keccak,
                PasswordMd5Hash = md5,
                Role = Roles.Simple
            };
            _unit.User.Save(_user);           
        }

        [Test]
        public void ShouldReturnCorrectMembershipUserWithGivenEmail()
        {
            // act
            var user = _membershipProvider.GetUser(_user.Email, true);

            // assert
            user.Should().NotBeNull();
            user.UserName.Should().Be(string.Format("{0} {1}", _user.FirstName, _user.LastName));
            user.Email.Should().Be(_user.Email);           
        }

        [Test]
        public void ShouldReturnCorrectUserNameWithGivenEmail()
        {
            // act & assert
            string
                .Format("{0} {1}", _user.FirstName, _user.LastName)
                .Should()
                .Be(_membershipProvider.GetUserNameByEmail(_user.Email));
        }

        [Test]
        public void ShouldReturnTrueIfUsersPairEmailAndPasswordIsValid()
        {
            // act & assert
            _membershipProvider.ValidateUser(_user.Email, _userPassword).Should().BeTrue();
        }
    }
}
