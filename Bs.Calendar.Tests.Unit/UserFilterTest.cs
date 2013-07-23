using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture] 
    class UserFilterTest
    {
        private UserService _userService;
        private List<User> _users;

        [TestFixtureSetUp]
        public void Setup()
        {
            _users = new List<User>
            {
                new User {Email = "12345@gmail.com", FirstName = "Saveli", LastName = "Bondini"},
                new User {Email = "5678@gmail.com", FirstName = "Dima", LastName = "Rossi"},
                new User {Email = "9999@gmail.com", FirstName = "Dima", LastName = "Prohorov"}
            };

            var moq = new Mock<IUserRepository>();
            moq.Setup(m => m.Load()).Returns(_users.AsQueryable());

            DiMvc.Register();
            Resolver.RegisterInstance<IUserRepository>(moq.Object);
            _userService = Resolver.Resolve<UserService>();
        }

        
        [Test]
        public void Find_Should_Return_One_User_When_Filter_Has_Email()
        {
            //arrange
            var testUser = _users[0];

            //act
            var users = _userService.Find(testUser.Email).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(1);
            users.First().Email.ShouldBeEquivalentTo(testUser.Email);
            users.First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
        }


        [Test]
        public void Find_Return_User_When_Filter_By_Name() 
        {
            //arrange
            var testUser = _users[0];

            //act
            var users = _userService.Find(testUser.FirstName).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(1);
            users.First().Email.ShouldBeEquivalentTo(testUser.Email);
            users.First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
        }

        [Test]
        public void Find_Return_Many_Users_When_Filter_By_Similar_Name() {
            //arrange
            var testUser = _users[1];

            //act
            var users = _userService.Find(testUser.FirstName).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(2);
            users.First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
            users.Skip(1).First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
        }


        [Test]
        public void Find_Return_No_User_When_Filter_By_Nonexistent_Email() {
            //arrange
            var testUser = new User {Email = "00000@gmail.com"};

            //act
            var users = _userService.Find(testUser.Email).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(0);
        }


        [Test]
        public void Find_Return_No_User_When_Filter_By_Nonexistent_Name() {
            //arrange
            var testUser = new User { FirstName = "Alex" };

            //act
            var users = _userService.Find(testUser.FirstName).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(0);
        }


        [Test]
        public void Find_Return_All_Users_When_Filter_By_Empty_String() {
            //arrange
            var emptyString = string.Empty;

            //act
            var users = _userService.Find(emptyString).Users;
            //assert
            users.Count().ShouldBeEquivalentTo(_users.Count);
        }

        [Test]
        public void Find_Return_All_Users_When_Filter_By_Null_String() {
            //arrange
            string nullString = null;

            //act
            var users = _userService.Find(nullString).Users;
            //assert
            users.Count().ShouldBeEquivalentTo(_users.Count);
        }
    }
}
