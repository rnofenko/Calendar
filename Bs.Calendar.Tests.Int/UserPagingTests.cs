using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bs.Calendar.Core;
using System.Web.Mvc;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;
using FluentAssertions;
using Moq;
using NUnit.Framework;

using Bs.Calendar.Tests.Unit.FakeObjects;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class UserPagingTest
    {
        private RepoUnit _repoUnit;
        private UsersController _usersController;
        private FakeConfig _config;

        private List<User> _users;

        [TestFixtureSetUp]
        public void SetUp()
        {
            DiMvc.Register();
            Ioc.RegisterType<IConfig, FakeConfig>();

            _config = Ioc.Resolve<IConfig>() as FakeConfig;

            _users = new List<User>()
                         {
                             new User { Email = "aaa@bbb.com", FullName = "aaa ddd", FirstName = "aaa", LastName = "ddd"},
                             new User { Email = "ccc@ddd.com", FullName = "aaa bbb", FirstName = "aaa", LastName = "bbb"}
                         };

            _repoUnit = new RepoUnit();
            _users.ForEach(_repoUnit.User.Save);

            var userService = new UserService(_repoUnit, null);
            Ioc.RegisterInstance<UserService>(userService);                    

            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);

            _usersController = new UsersController(userService) { ControllerContext = mock.Object };

            _config.PageSize = 1;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _users.ForEach(_repoUnit.User.Delete);
        }

        //[Test]
        //public void Can_Paginate_Users()
        //{
        //    //act
        //    var usersView = _usersController.List(new UserFilterVm()) as PartialViewResult;
        //    var users = usersView.Model as UsersVm;

        //    //assert
        //    users.Users.Count().ShouldBeEquivalentTo(_config.PageSize);
        //}

        //[Test,
        //TestCase("FullName", new[] { "aaa bbb", "aaa ddd" }),
        //TestCase("FullName desc", new[] { "aaa ddd", "aaa bbb" })]
        //public void Should_sort_users_by_name(string sortByField, string[] expectedUsers)
        //{
        //    //act
        //    var usersView = _usersController.List(new UserFilterVm { SortByField = sortByField }) as PartialViewResult;
        //    var users = usersView.Model as UsersVm;

        //    //assert
        //    users.Users.Select(u => u.FullName).ShouldAllBeEquivalentTo(expectedUsers);
        //}
    }
}
