using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class UsersControllerTest
    {
        private RepoUnit _repoUnit;
        private UserService _userService;
        
        private int _lastDbRecordId;

        [TearDown]
        public void TearDown()
        {
            _repoUnit.User.Load(user => user.Id > _lastDbRecordId).ToList().ForEach(deletedUser => _repoUnit.User.Delete(deletedUser));
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();

            Ioc.RegisterType<IUserRepository, UserRepository>();

            _repoUnit = Ioc.Resolve<RepoUnit>();

            _userService = new UserService(_repoUnit, Ioc.Resolve<ContactService>());

            //Save last record id for post test database cleaning

            var lastRecord = _repoUnit.User.Load().ToList().LastOrDefault();
            _lastDbRecordId = lastRecord == null ? 0 : lastRecord.Id;
        }

        [Test]
        public void Should_throw_WarningException_When_add_user_with_existing_email()
        {
            // arrange

            string email = "bigbrother1984@gmail.com";

            _repoUnit.User.Save(new User
            {
                Email = email,
                Live = LiveStatuses.Active,
                ApproveState = ApproveStates.Approved
            });

            var userToAdd = new User
            {
                Email = email,
                Contacts = new Collection<Contact>()
            };

            // act

            Action action = () => _userService.SaveUser(new UserEditVm(userToAdd));

            // assert

            action.ShouldThrow<WarningException>().WithMessage(string.Format("User with email {0} already exists", email));
        }

        [Test]
        public void Should_throw_WarningException_When_add_user_with_email_provided_by_already_deleted_user()
        {
            // arrange

            string email = "bigbrother1984@gmail.com";

            _repoUnit.User.Save(new User
                                    {
                                        Email = email,
                                        Live = LiveStatuses.Deleted
                                    });

            var userToAdd = new User
            {
                Email = email,
                Contacts = new Collection<Contact>()
            };

            // act

            Action action = () => _userService.SaveUser(new UserEditVm(userToAdd));

            // assert
            
            action.ShouldThrow<WarningException>().WithMessage(string.Format("User with email {0} already exists", email));
        }

        [Test]
        public void Should_add_new_user_to_database()
        {
            // arrange

            string email = "bigbrother1984@gmail.com";

            var user = new User
                {
                    Email = email,
                    Contacts = new Collection<Contact>()
                };

            // act

            _userService.SaveUser(new UserEditVm(user));

            // assert

            _repoUnit.User.Load().ToList().Last().Email.ShouldBeEquivalentTo(email);
        }

        [Test]
        public void Should_modify_user_details_When_pass_existing_user_to_the_edit_view()
        {
            // arrange

            var modifiedUser = new User { Email = "bigbrother1984@gmail.com", Contacts = new Collection<Contact>() };
            _repoUnit.User.Save(modifiedUser);

            modifiedUser.Id = _repoUnit.User.Get(user => user.Email == modifiedUser.Email).Id;
            modifiedUser.Email = "elderbrother1884@gmail.com";

            // act

            new UsersController(_userService).Edit(new UserEditVm(modifiedUser), false);

            // assert

            _repoUnit.User.Get(modifiedUser.Id).Email.ShouldBeEquivalentTo(modifiedUser.Email);
        }

        [Test]
        public void Should_delete_user_from_database_When_specified_user_with_existing_email()
        {
            // arrange

            string email = "bigbrother1984@gmail.com";

            var deletedUser = new User { Email = email, Contacts = new Collection<Contact>() };

            _repoUnit.User.Save(deletedUser);

            // act

            new UsersController(_userService).Delete(new UserEditVm(deletedUser));

            // assert

            _repoUnit.User.Get(user => user.Email == email).Live.Should().Be(LiveStatuses.Deleted);
        }
    }
}