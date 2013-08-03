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
        private string _userSalt;
        private const int SALT_LENGTH = 128;

        [TestFixtureSetUp]
        public void SetUp()
        {
            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Ioc.Resolve<RepoUnit>();

            var cryptoProvider = new KeccakCryptoProvider();
            var saltProvider = new RandomSaltProvider();

            _membershipProvider = new CalendarMembershipProvider(cryptoProvider, saltProvider);
            _userPassword = "moriarty";
            _userSalt = saltProvider.GetSalt(SALT_LENGTH);

            var keccak = cryptoProvider.GetHashWithSalt(_userPassword, _userSalt);
            
            _user = new User
            {
                Email = "holmes@email.com",
                FirstName = "Sherlock",
                LastName = "Holmes",
                PasswordHash = keccak,
                PasswordSalt = _userSalt,
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
            user.UserName.Should().Be(_user.FullName);
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
