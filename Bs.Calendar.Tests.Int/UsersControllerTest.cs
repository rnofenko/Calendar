using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class UsersControllerTest
    {
        private RepoUnit _unit;
        private User _user;
        private UserService _userService;

        [TestFixtureSetUp]
        public void Setup()
        {
            _user = new User
                {
                    FirstName = "Winston",
                    LastName = "Smith",
                    Email = "bigbrother1984@gmail.com",
                    Role = Roles.Simple,
                    LiveState = LiveState.Active
                };
            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Ioc.Resolve<RepoUnit>();
            _userService = new UserService(_unit, null);
            _userService.SaveUser(new UserEditVm(_user));
            _user = _unit.User.Get(u => u.Email == _user.Email);
        }        

        [Test]
        public void CanNotAddNewUserWithExistingInTheDbEmail()
        {
            // arrange
            var userToAdd = new User
                {
                    FirstName = "Emmanuel",
                    LastName = "Goldstein",
                    Email = "bigbrother1984@gmail.com",
                    Role = Roles.Simple,
                    LiveState = LiveState.Active
                };

            // act
            Action action = () => _userService.SaveUser(new UserEditVm(userToAdd));

            // assert
            action.ShouldThrow<WarningException>().WithMessage(string.Format("User with email {0} already exists", userToAdd.Email));
        }

        [Test]
        public void CanNotAddNewUserWithExistingInTheDbEmailEvenIfUserWithThisEmaisIsDeleted()
        {
            // arrange
            _user.LiveState = LiveState.Deleted;
            _userService.UpdateUserState(_user.Id, LiveState.Deleted);

            var userToAdd = new User
            {
                FirstName = "Julia",
                LastName = "Htims",
                Email = "bigbrother1984@gmail.com",
                Role = Roles.Simple,
                LiveState = LiveState.Active
            };

            // act
            Action action = () => _userService.SaveUser(new UserEditVm(userToAdd));

            // assert
            action.ShouldThrow<WarningException>().WithMessage(string.Format("User with email {0} already exists", userToAdd.Email));
        }

        [Test]
        public void ShouldAddNewUserToTheDb()
        {
            // arrange             
            var quantaty = _userService.GetAllUsers().Count();
            var user = new User
                {
                    FirstName = "George",
                    LastName = "Orwell",
                    Email = "orwell.george@gmail.com",
                    Role = Roles.Simple,
                    BirthDate = null,
                    LiveState = LiveState.Active
                };

            // act
            _userService.SaveUser(new UserEditVm(user));

            // assert
            _userService.GetAllUsers().Count().Should().Be(quantaty + 1);
        }

        [Test]
        public void ShouldModifyUserInfoAndSaveToTheDb()
        {
            // arrange
            var user = _userService.GetAllUsers().Last();
            user.FirstName = "Big";
            user.LastName = "Brother";
            user.Email = "iamwatchingyou@gmail.com";
            user.Role = Roles.Simple;

            // act
            new UsersController(_userService).Edit(new UserEditVm(user), false);

            // assert
            var savedUser = _unit.User.Get(u =>
                (
                    u.FirstName == user.FirstName &&
                    u.LastName == user.LastName &&
                    u.Email == user.Email &&
                    u.Role == user.Role
                ));
            savedUser.Id.Should().Be(user.Id);
        }

        [Test]
        public void ShouldDeleteUserFromTheDb()
        {
            // arrange
            var userToDeleteVm = new UserEditVm
            {
                FirstName = " OBrien",
                LastName = "Agent",
                Email = "obrien@gmail.com",
                Role = Roles.Simple,
                LiveState = LiveState.Active
            };
            _userService.SaveUser(userToDeleteVm);

            var userToDelete = _unit.User.Get(u => u.Email == userToDeleteVm.Email);

            // act
            new UsersController(_userService).Delete(new UserEditVm(userToDelete));

            // assert
            _unit.User.Get(userToDelete.Id).LiveState.Should().Be(LiveState.Deleted);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var user1 = _unit.User.Get(user => user.Email == "bigbrother1984@gmail.com");
            var user2 = _unit.User.Get(user => user.Email == "orwell.george@gmail.com");
            var user3 = _unit.User.Get(user => user.Email == "iamwatchingyou@gmail.com");
            var changedUsersList = new List<User> { user1, user2, user3 };

            foreach (var user in changedUsersList.Where(user => user != null))
            {
                _unit.User.Delete(user);
            }
        }
    }
}