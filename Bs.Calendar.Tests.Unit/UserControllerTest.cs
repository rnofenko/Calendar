using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Core;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;
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
        private IUserRepository _repository;

        private List<User> _users;
        private FakeConfig _config;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            FakeDi.Register();

            _repository = new FakeUserRepository();
            Ioc.RegisterInstance<IUserRepository>(_repository);

            //Setup UserController dependencies
            _userController = Ioc.Resolve<UsersController>();

            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);
            _userController.ControllerContext = mock.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _users = new List<User>
            {
                new User {Email = "12345@gmail.com", FirstName = "Saveli", LastName = "Bondini", Contacts = new Collection<Contact>(), Role = Roles.Simple, Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved},
                new User {Email = "5678@gmail.com", FirstName = "Dima", LastName = "Rossi", Contacts = new Collection<Contact>(), Role = Roles.Simple, Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved},
                new User {Email = "9999@gmail.com", FirstName = "Dima", LastName = "Prohorov", Contacts = new Collection<Contact>(), Role = Roles.Simple, Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved}
            };

            _repository = Ioc.Resolve<IUserRepository>();
            _repository.Dispose();

            _users.ForEach(_repository.Save);

            _config = Config.Instance as FakeConfig;
            _config.PageSize = _users.Count;
        }

        [Test]
        public void Can_Not_Create_User_With_NonUnique_Email()
        {
            //arrange
            var testUserVm = new UserEditVm { Email = _users[0].Email, FirstName = "Alexandr" };

            //act
            _userController.Create(testUserVm);
            var userView = _userController.List(new UserFilterVm { NotApproved = true }) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Count(user => user.Email.Equals(testUserVm.Email)).ShouldBeEquivalentTo(1);
        }

        [Test,
        TestCase(""),
        TestCase(" "),
        TestCase("   "),
        TestCase("email"),
        TestCase("email@"),
        TestCase("@email"),
        TestCase("@email"),
        TestCase("@email.email"),
        TestCase("email@.email"),
        TestCase("email@email.  email"),
        TestCase("email@  email.email"),
        TestCase("email  @email.email"),
        TestCase("email  @email.  email"),
        TestCase("email@email.email")]
        public void Should_not_create_user_When_email_is_incorrect(string email)
        {
            //arrange
            var testUserVm = new UserEditVm { Email = email };

            //act
            _userController.Create(testUserVm);
            var userView = _userController.List(new UserFilterVm { NotApproved = true }) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().NotContain(user => user.Email.Equals(email));
        }

        [Test]
        public void Should_pass_user_model_with_correct_id_to_edit_view_When_redirect_to_edit_view()
        {
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
        public void Should_create_user()
        {
            //arrange
            var testUserVm = new UserEditVm { Email = "0000@gmail.com" };
            _config.PageSize = _users.Count + 1;

            //act
            _userController.Create(testUserVm);
            var userView = _userController.List(new UserFilterVm { Deleted = true, NotApproved = true }) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().HaveCount(_users.Count + 1);
            users.Skip(_users.Count).First().Email.ShouldBeEquivalentTo(testUserVm.Email);
        }

        [Test]
        public void Should_save_edit_changes_When_editing_user()
        {
            //arrange
            var testUserVm = new UserEditVm { UserId = _users[1].Id, Email = "email@email.mail", Role = Roles.Admin };

            //act
            _userController.Edit(testUserVm, false);
            var userView = _userController.List(new UserFilterVm { NotApproved = true }) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().Contain(user => user.Email.Equals(testUserVm.Email) && user.Id == testUserVm.UserId);
        }

        [Test]
        public void Should_delete_user_with_correct_id_When_deleting_user()
        {
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
        public void Should_delete_user()
        {
            //arrange
            var testUserVm = new UserEditVm(_users[2]);

            //act
            _userController.Delete(testUserVm);
            var userView = _userController.List(new UserFilterVm { NotApproved = true }) as PartialViewResult;
            var users = (userView.Model as UsersVm).Users;

            //assert
            users.Should().NotContain(user => user.Email.Equals(testUserVm.Email));
        }
    }
}
