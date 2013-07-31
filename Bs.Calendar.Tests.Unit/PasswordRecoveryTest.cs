using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Rules;
using FluentAssertions;
using Moq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Tests.Unit.FakeObjects;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    public class PasswordRecoveryTest
    {
        private List<User> _users;
        private AccountService _accountService;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _users = new List<User>()
            {
                new User {Id = 1, Email = "12345@gmail.com", FirstName = "Saveli", LastName = "Bondini", Role = Roles.None, LiveState = LiveState.Active},
                new User {Id = 2, Email = "5678@gmail.com", FirstName = "Dima", LastName = "Rossi", Role = Roles.None, LiveState = LiveState.Active},
                new User {Id = 3, Email = "9999@gmail.com", FirstName = "Dima", LastName = "Prohorov", Role = Roles.None, LiveState = LiveState.Active}
            };

            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();

            var repoUnit = new RepoUnit();
            _users.ForEach(u => repoUnit.User.Save(u));
            _accountService = Resolver.Resolve<AccountService>();
        }

        [Test]
        [ExpectedException(typeof(WarningException))]
        public void Should_Throw_Exception_On_NonExistent_Email()
        {
            //arrange
            var alienEmail = "alien@gmail.com";

            //act
            _accountService.PasswordRecovery(alienEmail, "");
        }

        [Test]
        public void Can_Find_Existent_Email()
        {
            //arrange
            var existentUser = _users[0];

            //act
            _accountService.PasswordRecovery(existentUser.Email, "localhost/");

            //assert
            existentUser.PassRecovery.Should().NotBeNull();
            existentUser.PassRecovery.PasswordKeccakHash.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Can_Form_Recovery_Link_And_Send_Email()
        {
            //arrange
            var mailMessage = new MailMessage();
            var url = "localhost/";
            var moq = new Mock<EmailSender>();

            moq.Setup(e => e.SendEmail(It.IsAny<MailMessage>()))
               .Callback<MailMessage>(m => mailMessage = m);
            Resolver.RegisterInstance<EmailSender>(moq.Object);

            //act
            _accountService.PasswordRecovery(_users[0].Email, url);
            var expectedUrl = string.Format("{0}PasswordReset/{1}/{2}", url, _users[0].Id,_users[0].PassRecovery.PasswordKeccakHash);

            //assert
            moq.Verify(e => e.SendEmail(It.IsAny<MailMessage>()), Times.Once());
            mailMessage.Body.Should().Contain(expectedUrl);
            mailMessage.To.Contains(new MailAddress(_users[0].Email)).Should().BeTrue();
        }

        [Test]
        [ExpectedException(typeof(WarningException))]
        public void Should_Throw_Exception_On_Invalid_Token()
        {
            //arrange
            var user = _users[0];
            user.PassRecovery = new PassRecovery {Date = DateTime.Now, PasswordKeccakHash = "valid"};

            //act
            _accountService.CheckToken(user.Id, "invalid");
        }

        [Test]
        [ExpectedException(typeof(WarningException))]
        public void Should_Throw_Exception_On_Invalid_Link_Date() {
            //arrange
            var user = _users[0];
            user.PassRecovery = new PassRecovery { Date = DateTime.Now - new TimeSpan(24, 0, 0), PasswordKeccakHash = "valid" };

            //act
            _accountService.CheckToken(user.Id, "valid");
        }

        [Test]
        public void Should_Return_Model_On_Valid_Token_And_Date()
        {
            //arrange
            var user = _users[0];
            user.PassRecovery = new PassRecovery { Date = DateTime.Now, PasswordKeccakHash = "valid" };

            //act
            var model = _accountService.CheckToken(user.Id, "valid");

            //assert
            model.Should().NotBeNull();
            model.Email.ShouldBeEquivalentTo(user.Email);
        }

        [Test]
        public void Should_Change_Password_And_Clear_Hash()
        {
            //arrange
            var user = _users[0];
            user.PassRecovery = new PassRecovery { Date = DateTime.Now, PasswordKeccakHash = "valid" };

            //act
            _accountService.ResetPassword(new AccountVm { Email = user.Email, Password = "1234567"});

            //assert
            user.PassRecovery.PasswordKeccakHash.Should().BeEmpty();
            user.PasswordKeccakHash.ShouldBeEquivalentTo(Resolver.Resolve<ICryptoProvider>().GetHashWithSalt("1234567"));
        }
    }
}
