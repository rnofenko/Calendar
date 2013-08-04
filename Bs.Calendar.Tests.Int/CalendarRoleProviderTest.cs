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
    class CalendarRoleProviderTest
    {
        private CalendarRoleProvider _roleProvider;

        private RepoUnit _unit;

        private User _user;
        private string _userPassword;
        private string _userSalt;
        private const int SALT_LENGTH = 128;

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Ioc.Resolve<RepoUnit>();

            _roleProvider = new CalendarRoleProvider();

            var cryptoProvider = new KeccakCryptoProvider();
            var saltProvider = new RandomSaltProvider();
            var salt = saltProvider.GetSalt(SALT_LENGTH);

            _userPassword = "IrenAdler";
            var keccak = cryptoProvider.GetHashWithSalt(_userPassword, salt);

            _user = new User
            {
                Email = "holmes@email.com",
                FirstName = "Sherlock",
                LastName = "Holmes",
                PasswordHash = keccak,
                Role = Roles.Simple
            };
            _unit.User.Save(_user);                       
        }

        [Test]
        public void ShouldReturnTrueIfUserInRole()
        {
            // act & assert
            _roleProvider.IsUserInRole(_user.Email, _user.Role.ToString()).Should().BeTrue();
        }

        [Test]
        public void ShouldReturnTrueIfUsersRoleExists()
        {
            // act & assert
            _roleProvider.RoleExists(_user.Role.ToString()).Should().BeTrue();
            _roleProvider.RoleExists("FakeRole").Should().BeFalse();
        }

        [TestCase("Admin"),
        TestCase("Simple"),
        TestCase("None")]
        public void ShouldReturnEmailsOfTheUsersWithGivenRole(string role)
        {
            // arrange
            var usersWithGivenRole = _unit.User.Get(u => u.Role.ToString() == role);
            if (usersWithGivenRole == null)
            {
                // act & assert
                _roleProvider.GetUsersInRole(role).Should().BeNull();
            }
            else
            {
                var usersEmails = new [] {usersWithGivenRole.Email};

                // act
                var usersEmailsRoleProvider = _roleProvider.GetUsersInRole(role);

                // assert
                usersEmailsRoleProvider.Should().BeEquivalentTo(usersEmails);
            }
        }
    }
}
