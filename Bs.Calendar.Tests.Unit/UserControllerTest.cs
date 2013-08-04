using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Core;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class UserControllerTest
    {
        private UsersController _userController;
        private List<User> _users;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _users = new List<User>
            {
                new User {Id = 1, Email = "12345@gmail.com", FirstName = "Saveli", LastName = "Bondini", Role = Roles.None, LiveState = LiveState.Active},
                new User {Id = 2, Email = "5678@gmail.com", FirstName = "Dima", LastName = "Rossi", Role = Roles.None, LiveState = LiveState.Active},
                new User {Id = 3, Email = "9999@gmail.com", FirstName = "Dima", LastName = "Prohorov", Role = Roles.None, LiveState = LiveState.Active}
            };

            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);

            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();

            var repoUnit = new RepoUnit();
            _users.ForEach(user => repoUnit.User.Save(user));

            Ioc.RegisterInstance<RepoUnit>(repoUnit);
            _userController = Ioc.Resolve<UsersController>();
            _userController.ControllerContext = mock.Object;
        }

        [Test]
        public void Can_View_Details_Of_User()
        {
            //arrange
            var testUser = _users[0];

            //act
            var viewResult = _userController.Details(testUser.Id) as ViewResult;
            var userEditVm = viewResult.Model as UserEditVm;

            //assert
            userEditVm.UserId.ShouldBeEquivalentTo(testUser.Id);
            userEditVm.Email.ShouldBeEquivalentTo(testUser.Email);
        }

        [Test]
        public void Can_Create_Users() {
            //arrange
            var testUserVm = new UserEditVm(0, "Alexandr", "Fomkin", "0000@gmail.com", Roles.None, null, LiveState.Active);

            //act
            _userController.Create(testUserVm);
            var userView = _userController.List(new PagingVm()) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().Contain(user => user.Email.Equals(testUserVm.Email));
        }

        [Test]
        public void Can_Not_Create_User_With_NonUnique_Email() {
            //arrange
            var testUserVm = new UserEditVm { Email = _users[0].Email, FirstName = "Alexandr" };

            //act
            _userController.Create(testUserVm);
            var userView = _userController.List(new PagingVm()) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Count(user => user.Email.Equals(testUserVm.Email)).ShouldBeEquivalentTo(1);
        }

        [Test]
        public void Can_Not_Create_User_With_Wrong_Email() {
            //arrange
            var testUserVm = new UserEditVm { Email = "wrong", FirstName = "Alexandr" };

            //act
            _userController.Create(testUserVm);
            var userView = _userController.List(new PagingVm()) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().NotContain(user => user.Email.Equals(testUserVm.Email));
        }

        [Test]
        public void Edit_Id_Method_Works_With_Correct_User() {
            //arrange
            var testUser = _users[1];

            //act
            var viewResult = _userController.Edit(testUser.Id) as ViewResult;
            var userEditVm = viewResult.Model as UserEditVm;

            //assert
            userEditVm.UserId.ShouldBeEquivalentTo(testUser.Id);
            userEditVm.Email.ShouldBeEquivalentTo(testUser.Email);
        }

        [Test]
        public void Can_Edit_User() {
            //arrange
            var testUserVm = new UserEditVm(_users[1].Id, "Toto", "Koko", "ggggg@gmail.com", Roles.Admin, null, LiveState.Active);

            //act
            _userController.Edit(testUserVm, false);
            var userView = _userController.List(new PagingVm()) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().Contain(user => user.Email.Equals(testUserVm.Email));
            users.Should().Contain(user => user.FirstName.Equals(testUserVm.FirstName));
            users.Should().Contain(user => user.LastName.Equals(testUserVm.LastName));
            users.Should().Contain(user => user.Role.Equals(testUserVm.Role));
        }

        [Test]
        public void Delete_Id_Method_Works_With_Correct_User() {
            //arrange
            var testUser = _users[1];

            //act
            var viewResult = _userController.Delete(testUser.Id) as ViewResult;
            var userEditVm = viewResult.Model as UserEditVm;

            //assert
            userEditVm.UserId.ShouldBeEquivalentTo(testUser.Id);
            userEditVm.Email.ShouldBeEquivalentTo(testUser.Email);
        }

        [Test]
        public void Can_Delete_User() {
            //arrange
            var testUserVm = new UserEditVm(_users[2]);

            //act
            _userController.Delete(testUserVm);
            var userView = _userController.List(new PagingVm()) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().NotContain((user => user.Email.Equals(testUserVm.Email)));
        }
    }
}
