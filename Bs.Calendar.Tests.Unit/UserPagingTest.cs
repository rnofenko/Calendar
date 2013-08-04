using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class UserPagingTest
    {
        private UserService _userService;
        private List<User> _users;

        [TestFixtureSetUp]
        public void Setup() {
            _users = new List<User>
            {
                new User {Email = "12345@gmail.com", FirstName = "Saveli", LastName = "Bondini", LiveState = LiveState.Active},
                new User {Email = "5678@gmail.com", FirstName = "Dima", LastName = "Rossi", LiveState = LiveState.Active},
                new User {Email = "9999@gmail.com", FirstName = "Dima", LastName = "Prohorov", LiveState = LiveState.Active},
                new User {Email = "0000@gmail.com", FirstName = "Alex", LastName = "Sinov", LiveState = LiveState.Active}
            };
            _users.ForEach(user => user.FullName = string.Format("{0} {1}", user.FirstName, user.LastName));

            var moq = new Mock<IUserRepository>();
            moq.Setup(m => m.Load()).Returns(_users.AsQueryable());

            DiMvc.Register();
            Ioc.RegisterInstance<IUserRepository>(moq.Object);
            _userService = Ioc.Resolve<UserService>();
        }

        [Test]
        public void Can_Paginate_Users()
        {
            //arrange
            _userService.PageSize = 2;

            //act
            var usersPage1 = _userService.RetreiveList(new PagingVm {Page = 1}).Users;
            var usersPage2 = _userService.RetreiveList(new PagingVm {Page = 2}).Users;

            //assert
            usersPage1.Count().ShouldBeEquivalentTo(_userService.PageSize);
            usersPage2.Count().ShouldBeEquivalentTo(_userService.PageSize);

            usersPage1.First().ShouldBeEquivalentTo(_users[0]);
            usersPage1.Last().ShouldBeEquivalentTo(_users[1]);

            usersPage2.First().ShouldBeEquivalentTo(_users[2]);
            usersPage2.Last().ShouldBeEquivalentTo(_users[3]);
        }

        [Test]
        public void Can_Sort_Users() 
        {
            //arrange
            _userService.PageSize = _users.Count;

            //act
            var users = _userService.RetreiveList(new PagingVm {SortByStr = "Name", Page = 1}).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(_userService.PageSize);
            users.First().ShouldBeEquivalentTo(_users[3]);
            users.Last().ShouldBeEquivalentTo(_users[0]);
        }

        [Test]
        public void Can_Sort_Users_On_Multiple_Pages() 
        {
            //arrange
            _userService.PageSize = 2;

            //act
            var usersPage1 = _userService.RetreiveList(new PagingVm { SortByStr = "Name", Page = 1 }).Users;
            var usersPage2 = _userService.RetreiveList(new PagingVm { SortByStr = "Name", Page = 2 }).Users;

            //assert
            usersPage1.First().ShouldBeEquivalentTo(_users[3]);
            usersPage2.Last().ShouldBeEquivalentTo(_users[0]);
        }

        [Test]
        public void Can_Paginate_Sort_And_Search_Users() 
        {
            //arrange
            _userService.PageSize = 2;

            //act
            var usersPage2 = _userService.RetreiveList(new PagingVm
            {
                SearchStr = "Dima", SortByStr = "Name", Page = 2
            }).Users;

            //assert
            usersPage2.Count().ShouldBeEquivalentTo(_userService.PageSize);
            usersPage2.First().LastName.ShouldBeEquivalentTo("Prohorov");
        }
    }
}
