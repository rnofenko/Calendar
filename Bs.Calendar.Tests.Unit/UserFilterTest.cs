using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Tests.Unit.FakeObjects;
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

            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();
            
            var repoUnit = Resolver.Resolve<RepoUnit>();
            _users.ForEach(user => repoUnit.User.Save(user));

            _userService = new UserService(repoUnit);
        }

        
        [Test]
        public void Find_should_return_one_user_When_filter_has_email()
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
        public void Find_return_user_When_filter_by_Name() 
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
        public void Find_return_many_users_When_filter_by_similar_Name() {
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
        public void Find_return_no_user_When_filter_by_Nonexistent_Email() {
            //arrange
            var testUser = new User {Email = "00000@gmail.com"};

            //act
            var users = _userService.Find(testUser.Email).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(0);
        }


        [Test]
        public void Find_return_no_user_When_filter_by_Nonexistent_Name() {
            //arrange
            var testUser = new User { FirstName = "Alex" };

            //act
            var users = _userService.Find(testUser.FirstName).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(0);
        }


        [Test]
        public void Find_return_all_users_When_filter_by_empty_String() {
            //arrange
            var emptyString = string.Empty;

            //act
            var users = _userService.Find(emptyString).Users;
            //assert
            users.Count().ShouldBeEquivalentTo(_users.Count);
        }
    }
}
