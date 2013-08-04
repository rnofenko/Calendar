using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Core;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class UserFrameTest
    {
        private RepoUnit _unit;
        private UsersController _usersController;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);

            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, UserRepository>();

            _unit = new RepoUnit();

            _unit.User.Save(new User { Email = "aaa@bbb.com", FirstName = "aaa", LastName = "bbb", LiveState = LiveState.Active, Role = Roles.Admin});
            _unit.User.Save(new User { Email = "ccc@ddd.com", FirstName = "ccc", LastName = "ddd", LiveState = LiveState.Active, Role = Roles.Simple});

            _usersController = new UsersController(new UserService(_unit));
            _usersController.ControllerContext = mock.Object;
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
        public void Can_Provide_All_Users()
        {
            //act
            var usersView = _usersController.List(new PagingVm()) as PartialViewResult;
            var users = usersView.Model as UsersVm;

            //assert
            users.Users.Count().Should().BeGreaterOrEqualTo(2);
        }

        [Test]
        public void Can_Search_Users()
        {
            //arrange
            var pagingVm = new PagingVm();
            pagingVm.SearchStr = "ccc@ddd.com";

            //act
            var usersView = _usersController.List(pagingVm) as PartialViewResult;
            var users = usersView.Model as UsersVm;
            var user = users.Users.First();

            //assert
            user.Email.ShouldBeEquivalentTo(pagingVm.SearchStr);
        }

        [Test]
        public void CanSearchUsersByRole()
        {
            //arrange
            var pagingVm = new PagingVm { SearchStr = "Admin" };

            //act
            var usersView = _usersController.List(pagingVm) as PartialViewResult;
            var users = usersView.Model as UsersVm;

            //assert
            users.Users.Should().Contain(u => u.Role.ToString() == "Admin");
        }
    }
}
