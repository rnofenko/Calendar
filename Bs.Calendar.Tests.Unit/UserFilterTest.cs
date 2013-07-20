using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture] 
    class UserFilterTest
    {
        private List<User> _users;

        [TestFixtureSetUp]
        public void Setup()
        {
            //arrange
            _users = new List<User>
            {
                new User {Email = "bondinis@gmail.com", FirstName = "Saveli", LastName = "Bondini"},
                new User {Email = "jango@gmail.com", FirstName = "Jango", LastName = "Rossi"},
                new User {Email = "kolo@gmail.com", FirstName = "Dima", LastName = "Prohorov"}
            };
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
