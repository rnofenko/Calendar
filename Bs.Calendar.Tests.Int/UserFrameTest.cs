using System;
using System.Linq;
using System.Web.Mvc;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture] 
    class UserFrameTest
    {
        private RepoUnit _unit;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _unit = new RepoUnit();

            _unit.User.Save(new User {Email = "aaa@bbb.com", FirstName = "aaa", LastName = "bbb"});
            _unit.User.Save(new User { Email = "ccc@ddd.com", FirstName = "ccc", LastName = "ddd" });
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var user1 = _unit.User.Get(user => user.FirstName.Equals(
                                       "aaa", StringComparison.InvariantCulture));
            var user2 = _unit.User.Get(user => user.FirstName.Equals(
                                       "ccc", StringComparison.InvariantCulture));
            _unit.User.Delete(user1);
            _unit.User.Delete(user2);
        }

        [Test]
        public void CanDisplayUsers()
        {
            //arrange
            var usersController = Core.Resolver.Resolve<UsersController>();

            //act
            var usersView = usersController.Index() as ViewResult;
            var users = (UsersVm)usersView.Model;

            //assert
            Assert.GreaterOrEqual(users.Users.Count(), 2);
        }

        [Test]
        public void CanFilterUsers() 
        {
            //arrange
            var usersController = Core.Resolver.Resolve<UsersController>();

            //act
            var usersView = usersController.List("ccc@ddd.com") as PartialViewResult;
            
            var users = (UsersVm)usersView.Model;
            var user = users.Users.First();

            //assert
            Assert.AreEqual("ccc@ddd.com", user.Email);
            Assert.AreEqual("ccc", user.FirstName);
            Assert.AreEqual("ddd", user.LastName);
        }
    }
}
