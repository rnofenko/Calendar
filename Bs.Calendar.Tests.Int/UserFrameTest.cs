using System;
using System.Collections.Generic;
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
using Bs.Calendar.Tests.Unit.FakeObjects;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class UserFrameTest
    {
        private RepoUnit _repoUnit;
        private UsersController _usersController;
        private List<User> _users;

        private int _lastDbRecordId;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _users = new List<User>()
                         {
                             new User { Email = "aaa@bbb.com", FullName = "aaa ddd", FirstName = "aaa", LastName = "ddd", Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved, Role = Roles.Simple },
                             new User { Email = "ccc@ddd.com", FullName = "aaa bbb", FirstName = "aaa", LastName = "bbb", Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved, Role = Roles.Admin }
                         };

            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);

            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, UserRepository>();

            _repoUnit = new RepoUnit();

            var lastRecord = _repoUnit.User.Load();
            _lastDbRecordId = !lastRecord.Any() ? 0 : lastRecord.Max(user => user.Id);

            _users.ForEach(user => _repoUnit.User.Save(user));

            var service = new UserService(_repoUnit, null)
                              {
                                  PageSize = lastRecord.Count() + _users.Count //Prevent paging
                              };

            _usersController = new UsersController(service);

            _usersController.ControllerContext = mock.Object;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _users.ForEach(user => _repoUnit.User.Delete(user));
        }

        [Test]
        public void Should_return_all_users_When_no_filter_is_applied()
        {
            //act

            var usersView = _usersController.List(new PagingVm(true, true, true, true, true, true)) as PartialViewResult;
            var users = usersView.Model as UsersVm;

            //assert

            users.Users.Count(user => user.Id > _lastDbRecordId).ShouldBeEquivalentTo(_users.Count);
        }

        [Test]
        public void Should_return_user_When_searching_exisiting_user_by_his_email()
        {
            //arrange

            var searchedUser = _users[0];
            var pagingVm = new PagingVm(true, true, true, true, true, true) { SearchStr = searchedUser.Email };

            //act

            var usersView = _usersController.List(pagingVm) as PartialViewResult;
            var users = usersView.Model as UsersVm;

            //assert

            var foundUsers = users.Users.Where(userFromView => userFromView.Id > _lastDbRecordId);
            
            foundUsers.Count().ShouldBeEquivalentTo(1);
            foundUsers.First().Email.ShouldBeEquivalentTo(searchedUser.Email);
        }

        [Test,
        TestCase(Roles.Simple),
        TestCase(Roles.Admin)]
        public void Should_return_all_users_with_corresponding_roles_When_search_by_role(Roles roleToSearch)
        {
            //arrange

            var pagingVm = new PagingVm(true, true, true, true, true, true) { SearchStr = roleToSearch.ToString() };

            //act

            var usersView = _usersController.List(pagingVm) as PartialViewResult;
            var users = usersView.Model as UsersVm;

            //assert

            var foundUsers = users.Users.Where(user => user.Id > _lastDbRecordId);
            foundUsers.Count().ShouldBeEquivalentTo(1);
            foundUsers.First().Role.ShouldBeEquivalentTo(roleToSearch);
        }
    }
}
