using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
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
            //act
            var userService = new UserService(null);
            var filteredUsers = userService.Find(_users, "bondinis@gmail.com").ToList();
            var result = filteredUsers[0];

            //assert
            Assert.AreEqual("bondinis@gmail.com", result.Email);
            Assert.AreEqual("Saveli", result.FirstName);
            Assert.AreEqual("Bondini", result.LastName);
        }

        [Test]
        public void CanFilterByName() 
        {
            //act
            var userService = new UserService(null);
            var filteredUsers = userService.Find(_users, "Jango Rossi").ToList();
            var result = filteredUsers[0];

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
            var filteredUsers = userService.Find(_users, "").ToList();

            //assert
            Assert.AreEqual(3, filteredUsers.Count);
        }

        [Test]
        public void CanFilterByNonexistentUser() {
            //act
            var userService = new UserService(null);
            var filteredUsers = userService.Find(_users, "Oleg Beloy").ToList();

            //assert
            Assert.AreEqual(0, filteredUsers.Count);
        }
    }
}
