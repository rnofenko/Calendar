using System.Linq;
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

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Resolver.Resolve<RepoUnit>();

            _roleProvider = new CalendarRoleProvider();

            var crypto = new CryptoProvider();
            _userPassword = "IrenAdler";
            var keccak = crypto.GetKeccakHash(_userPassword);
            var md5 = crypto.GetMd5Hash(_userPassword);

            _user = new User
            {
                Email = "holmes@email.com",
                FirstName = "Sherlock",
                LastName = "Holmes",
                PasswordKeccakHash = keccak,
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
