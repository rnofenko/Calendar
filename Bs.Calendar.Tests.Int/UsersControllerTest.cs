using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
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
            _unit = new RepoUnit();
            _userService = new UserService(_unit);

            _user = new User
                {
                    FirstName = "New",
                    LastName = "User",
                    Email = "newuser@gmail.com",
                    Role = Roles.Simple
                };
            _userService.SaveUser(new UserEditVm(_user));
        }

        [Test]
        public void CanNotAddNewUserWithExistingInTheDbEmail()
        {
            // arrange
            var userToAdd = new User
                {
                    FirstName = "Same",
                    LastName = "Email",
                    Email = "newuser@gmail.com",
                    Role = Roles.Simple
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
                    FirstName = "NewOne",
                    LastName = "AnotherUser",
                    Email = "newoneuser@gmail.com",
                    Role = Roles.None
                };

            // act
            _userService.SaveUser(new UserEditVm(user));

            // assert
            _userService.GetAllUsers().Count().Should().Be(quantaty + 1);
        }

        [Test]
        public void IsShownDetailsAppliesToSelectedUser()
        {
            // arrange
            var user = _userService.GetAllUsers().First();

            // act
            var viewResult = new UsersController(_userService).Details(user.Id) as ViewResult;
            var model = viewResult.Model as UserEditVm;

            // assert
            model.ShouldBeEquivalentTo(new UserEditVm(user));            
        }

        [Test]
        public void ShouldModifyUserInfoAndSaveToTheDb()
        {
            // arrange
            var user = _userService.GetAllUsers().Last();
            user.FirstName = "Modyfied";
            user.LastName = "User";
            user.Email = "newemail@gmail.com";
            user.Role = Roles.None;

            // act
            var viewResult = new UsersController(_userService).Edit(new UserEditVm(user));

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
                FirstName = "User",
                LastName = "Del",
                Email = "deluser@gmail.com",
                Role = Roles.None
            };
            _userService.SaveUser(userToDeleteVm);

            var userToDelete = _unit.User.Get(u =>
                (
                    u.FirstName == userToDeleteVm.FirstName &&
                    u.LastName == userToDeleteVm.LastName &&
                    u.Email == userToDeleteVm.Email &&
                    u.Role == userToDeleteVm.Role
                ));

            // act
            var viewResult = new UsersController(_userService).Delete(new UserEditVm(userToDelete));

            // assert
            _unit.User.Get(userToDelete.Id).Should().Be(null);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var user1 = _unit.User.Get(user =>
                (
                    user.FirstName == "New" &&
                    user.LastName == "User" &&
                    user.Email == "newuser@gmail.com" &&
                    user.Role == Roles.Simple
                ));

            var user2 = _unit.User.Get(user =>
               (
                    user.FirstName == "NewOne" &&
                    user.LastName == "AnotherUser" &&
                    user.Email == "newoneuser@gmail.com" &&
                    user.Role == Roles.None
               ));

            var user3 = _unit.User.Get(user =>
                (
                    user.FirstName == "Modyfied" &&
                    user.LastName == "User" &&
                    user.Email == "newemail@gmail.com" &&
                    user.Role == Roles.None
                ));

            var changedUsersList = new List<User> {user1, user2, user3};

            foreach (var user in changedUsersList.Where(user => user!=null))
            {
                _unit.User.Delete(user);
            }
        }
    }
}