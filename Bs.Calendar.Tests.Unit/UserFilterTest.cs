using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Tests.Unit.FakeObjects;
using NUnit.Framework;
using FluentAssertions;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture] 
    class UserFilterTest
    {
        private List<User> _users;

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();

            //arrange
            _users = new List<User>
            {
                new User {Email = "bondinis@gmail.com", FirstName = "Saveli", LastName = "Bondini"},
                new User {Email = "jango@gmail.com", FirstName = "Jango", LastName = "Rossi"},
                new User {Email = "kolo@gmail.com", FirstName = "Dima", LastName = "Prohorov"}
            };
        }
        
        [Test]
        public void Find_should_return_one_user_When_filter_has_email()
        {
            const string EMAIL = "my@mail.com";
            const string NAME = "myname";

            var service = Resolver.Resolve<UserService>();
            service.SaveUser(new UserEditVm {Email = "12345@mail.com", FirstName = "dfsdfds"});
            service.SaveUser(new UserEditVm {Email = EMAIL, FirstName = NAME});
            service.SaveUser(new UserEditVm {Email = "423523@mail.com", FirstName = "fdgfdgfdgdfgdf"});

            var user = service.Find(EMAIL);

            user.Should().NotBeNull();
            user.Users.Count().Should().Be(1);
            user.Users.First().Email.Should().Be(EMAIL);
            user.Users.First().FirstName.Should().Be(NAME);
        }

        [Test]
        public void CanFilterByEmail()
        {
            ////arrange
            //var moq = new Mock<RepoUnit>();
            //moq.Setup(m => m.User).Returns(new UserRepository(null));

            ////act
            //var userService = new UserService(moq.Object);
            //var filteredUsers = userService.Find("bondinis@gmail.com");
            //var result = filteredUsers.Users.First();

            ////assert
            //Assert.AreEqual("bondinis@gmail.com", result.Email);
            //Assert.AreEqual("Saveli", result.FirstName);
            //Assert.AreEqual("Bondini", result.LastName);
        }

        [Test]
        public void CanFilterByName() 
        {
            //act
            var userService = new UserService(null);
            var filteredUsers = userService.Find("Jango Rossi");
            var result = filteredUsers.Users.First();

            //assert
            Assert.AreEqual("jango@gmail.com", result.Email);
            Assert.AreEqual("Jango", result.FirstName);
            Assert.AreEqual("Rossi", result.LastName);
        }

        [Test]
        public void CanFilterByEmptyString()
        {
            //act
            var userService = new UserService(null);
            var filteredUsers = userService.Find("");

            //assert
            Assert.AreEqual(3, filteredUsers.Users.Count());
        }

        [Test]
        public void CanFilterByNonexistentUser() {
            //act
            var userService = new UserService(null);
            var filteredUsers = userService.Find("Oleg Beloy");

            //assert
            Assert.AreEqual(0, filteredUsers.Users.Count());
        }
    }
}
