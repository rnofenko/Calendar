using System;
using System.Web.Security;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using Roles = Bs.Calendar.Models.Roles;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class CalendarRoleProviderTest
    {
        private RoleProvider _roleProvider;

        private RepoUnit _repoUnit;

        private User _user;
        private string _userPassword;
        private string _userSalt;
        private const int SALT_LENGTH = 128;

        [TestFixtureSetUp]
        public void Setup()
        {
            //Map interfaces for further modifications

            DiMvc.Register();
            
            Ioc.RegisterType<IUserRepository, UserRepository>();
            Ioc.RegisterType<ICryptoProvider, KeccakCryptoProvider>();
            Ioc.RegisterType<ISaltProvider, RandomSaltProvider>();
            Ioc.RegisterType<RoleProvider, CalendarRoleProvider>();

            _roleProvider = Ioc.Resolve<RoleProvider>();

            var cryptoProvider = Ioc.Resolve<ICryptoProvider>();
            var saltProvider = Ioc.Resolve<ISaltProvider>();

            _userPassword = "IrenAdler";
            _userSalt = saltProvider.GetSalt(SALT_LENGTH);
            var keccak = cryptoProvider.GetHashWithSalt(_userPassword, _userSalt);

            _user = new User
            {
                Email = "holmes@email.com",
                FirstName = "Sherlock",
                LastName = "Holmes",
                PasswordHash = keccak,
                Role = Roles.Simple
            };

            _repoUnit = Ioc.Resolve<RepoUnit>();
            _repoUnit.User.Save(_user);
        }

        [Test]
        public void Should_return_true_When_user_has_specified_role()
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

        [TestCase(Roles.Admin),
        TestCase(Roles.Simple),
        TestCase(Roles.None)]
        public void ShouldReturnEmailsOfTheUsersWithGivenRole(Roles role)
        {
            //arrange
            
            var usersWithGivenRole = _repoUnit.User.Get(u => u.Role == role);

            if (usersWithGivenRole == null)
            {
                //act & assert
                _roleProvider.GetUsersInRole(role.ToString()).Should().BeNull();
            }
            else
            {
                var usersEmails = new [] {usersWithGivenRole.Email};

                //act
                var usersEmailsRoleProvider = _roleProvider.GetUsersInRole(role.ToString());

                //assert
                usersEmailsRoleProvider.Should().BeEquivalentTo(usersEmails);
            }
        }
    }
}
