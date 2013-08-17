using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class UserPagingTest
    {
        private IUserRepository _repository;
        private UserService _userService;
        private FakeConfig _config;

        private List<User> _users;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            DiMvc.Register();

            Ioc.RegisterType<IConfig, FakeConfig>();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();

            _config = Config.Instance as FakeConfig;

            //Create and populate repository instance
            Ioc.RegisterInstance<IUserRepository>(new FakeUserRepository());
            _repository = Ioc.Resolve<IUserRepository>();

            _users = new List<User>
            {
                new User {Email = "abcd@gmail.com", FirstName = "Saveli", LastName = "Bondini", Role = Roles.Simple, ApproveState = ApproveStates.Approved, Live = LiveStatuses.Active},
                new User {Email = "cacd@gmail.com", FirstName = "Dima", LastName = "Rossi", Role = Roles.Simple, ApproveState = ApproveStates.Approved, Live = LiveStatuses.Active},
                new User {Email = "cabd@gmail.com", FirstName = "Dima", LastName = "Prohorov", Role = Roles.Simple, ApproveState = ApproveStates.Approved, Live = LiveStatuses.Active},
                new User {Email = "acbd@gmail.com", FirstName = "Alex", LastName = "Sinov", Role = Roles.Simple, ApproveState = ApproveStates.Approved, Live = LiveStatuses.Active}
            };

            _users.ForEach(_repository.Save);

            _userService = Ioc.Resolve<UserService>();
        }

        [Test]
        public void Can_Paginate_Users()
        {
            //arrange

            //var mockConfig = new Mock<IConfig>();
            //mockConfig.Setup(instance => instance.PageSize).Returns(pageSize);
            //Ioc.RegisterInstance<IConfig>(mockConfig.Object);

            _config.PageSize = 3;

            var expectedPage1Content = new User[] {_users[0], _users[1], _users[2]};
            var expectedPage2Content = new User[] { _users[3] };

            //act
            var usersPage1 = _userService.RetreiveList(new UserFilterVm {Page = 1}).Users;
            var usersPage2 = _userService.RetreiveList(new UserFilterVm {Page = 2}).Users;

            //assert
            usersPage1.ShouldAllBeEquivalentTo(expectedPage1Content);
            usersPage2.ShouldAllBeEquivalentTo(expectedPage2Content);
        }

        [Test,
        TestCase("Email", new[] { "abcd@gmail.com", "acbd@gmail.com", "cabd@gmail.com", "cacd@gmail.com" }),
        TestCase("Email desc", new[] { "cacd@gmail.com", "cabd@gmail.com", "acbd@gmail.com", "abcd@gmail.com" })]
        public void Should_sort_users_by_email(string sortBy, string[] expectedUsers)
        {
            //arrange
            _config.PageSize = _users.Count;

            //act
            var users = _userService.RetreiveList(new UserFilterVm { SortByField = sortBy }).Users;

            //assert
            users.Select(user => user.Email).ShouldAllBeEquivalentTo(expectedUsers);
        }

        [Test,
        TestCase("FullName", new[] { "Alex Sinov", "Dima Prohorov", "Dima Rossi", "Saveli Bondini" }),
        TestCase("FullName desc", new[] { "Saveli Bondini", "Dima Rossi", "Dima Prohorov", "Alex Sinov" })]
        public void Should_sort_users_by_full_name(string sortBy, string[] expectedUsers)
        {
            //arrange
            _config.PageSize = _users.Count;

            //act
            var users = _userService.RetreiveList(new UserFilterVm { SortByField = sortBy }).Users;

            //assert
            users.Select(user => user.FullName).ShouldAllBeEquivalentTo(expectedUsers);
        }

        [Test,
        TestCase("Email", new[] { "abcd@gmail.com", "acbd@gmail.com"}, new []{"cabd@gmail.com", "cacd@gmail.com" }),
        TestCase("Email desc", new[] { "cacd@gmail.com", "cabd@gmail.com"}, new[]{"acbd@gmail.com", "abcd@gmail.com" })]
        public void Should_sort_users_on_multiple_pages_by_email(string sortBy, string[] expectedPage1, string[] expectedPage2)
        {
            //arrange
            _config.PageSize = 2;

            //act
            var usersAtPage1 = _userService.RetreiveList(new UserFilterVm { SortByField = sortBy, Page = 1 }).Users;
            var usersAtPage2 = _userService.RetreiveList(new UserFilterVm { SortByField = sortBy, Page = 2 }).Users;

            //assert
            usersAtPage1.Select(user => user.Email).ShouldAllBeEquivalentTo(expectedPage1);
            usersAtPage2.Select(user => user.Email).ShouldAllBeEquivalentTo(expectedPage2);
        }

        [Test,
        TestCase("FullName", new[] { "Alex Sinov", "Dima Prohorov" }, new [] { "Dima Rossi", "Saveli Bondini" }),
        TestCase("FullName desc", new[] { "Saveli Bondini", "Dima Rossi"}, new [] { "Dima Prohorov", "Alex Sinov" })]
        public void Should_sort_users_on_multiple_pages_by_full_name(string sortBy, string[] expectedPage1, string[] expectedPage2)
        {
            //arrange
            _config.PageSize = 2;

            //act
            var usersAtPage1 = _userService.RetreiveList(new UserFilterVm { SortByField = sortBy, Page = 1 }).Users;
            var usersAtPage2 = _userService.RetreiveList(new UserFilterVm { SortByField = sortBy, Page = 2 }).Users;

            //assert
            usersAtPage1.Select(user => user.FullName).ShouldAllBeEquivalentTo(expectedPage1);
            usersAtPage2.Select(user => user.FullName).ShouldAllBeEquivalentTo(expectedPage2);
        }

        [Test]
        public void Can_Paginate_Sort_And_Search_Users()
        {
            //arrange
            _config.PageSize = 2;
            
            //act
            var userPage = _userService.RetreiveList(new UserFilterVm { SearchString = "Dima", SortByField = "FullName", Page = 1 }).Users;

            //assert
            userPage.Count().ShouldBeEquivalentTo(2);
            userPage.First().LastName.ShouldBeEquivalentTo("Prohorov");
        }
    }
}
