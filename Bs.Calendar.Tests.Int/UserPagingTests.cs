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
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class UserPagingTest
    {
        private RepoUnit _repoUnit;
        private UsersController _usersController;
        private int _pageSize;

        private List<User> _users;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _users = new List<User>()
                         {
                             new User { Email = "aaa@bbb.com", FullName = "aaa ddd", FirstName = "aaa", LastName = "ddd"},
                             new User { Email = "ccc@ddd.com", FullName = "aaa bbb", FirstName = "aaa", LastName = "bbb"}
                         };

            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);

            DiMvc.Register();

            _repoUnit = new RepoUnit();
            _users.ForEach(user => _repoUnit.User.Save(user));

            var userService = new UserService(_repoUnit, null);
            userService.PageSize = _pageSize = 1;

            _usersController = new UsersController(userService);
            _usersController.ControllerContext = mock.Object;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _users.ForEach(user => _repoUnit.User.Delete(user));
        }

        [Test]
        public void Can_Paginate_Users()
        {
            //act
            var usersView = _usersController.List(new PagingVm(false, false, false, true, true, true) {Page = _users.Count}) as PartialViewResult;
            var users = usersView.Model as UsersVm;

            //assert
            users.Users.Count().ShouldBeEquivalentTo(_pageSize);
        }

        [Test]
        public void Can_Sort_Users() 
        {
            //arrange

            var user = _repoUnit.User.Load().ToList()
                                            .Where(record => _users.Contains(record))
                                            .OrderBy(n => n.FirstName)
                                            .ThenBy(n => n.LastName)
                                            .First();

            //act

            var usersView = _usersController.List(new PagingVm(false, false, false, true, true, true) { Page = 1, SortByStr = "Name"}) as PartialViewResult;
            var users = usersView.Model as UsersVm;

            //assert

            users.Users.Count().ShouldBeEquivalentTo(_pageSize);
            users.Users.First().Email.ShouldBeEquivalentTo(user.Email);
        }
    }
}
