﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class AccountServiceTest
    {
        private AccountService _accountService;
        private RepoUnit _repoUnit;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var users = new List<User>
            {
                new User { Email = "12345@gmail.com", FullName = "Saveli Bondini", FirstName = "Saveli", 
                    LastName = "Bondini", BirthDate = DateTime.Now, Role = Roles.Simple,LiveState = LiveState.Active},
                    
                new User { Email = "00000@gmail.com", FullName = "Oleg Shepelev", FirstName = "Oleg", 
                    LastName = "Shepelev", BirthDate = DateTime.Now, Role = Roles.Simple,LiveState = LiveState.Active},
            };

            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();

            _repoUnit = new RepoUnit();
            Resolver.RegisterInstance<RepoUnit>(_repoUnit);

            users.ForEach(user => _repoUnit.User.Save(user));
            
            _accountService = Resolver.Resolve<AccountService>();
        }

        [Test]
        public void Can_Edit_Account_Info()
        {
            //arrange
            var testUserId = _repoUnit.User.Get(user => user.Email == "12345@gmail.com").Id;
            var testUserVm = new UserEditVm(testUserId, "Toto", "Koko", "54321@gmail.com", Roles.Simple, new DateTime(1991, 09, 20), LiveState.Active);

            //act
            _accountService.EditUser(testUserVm);
            var editedUser = _repoUnit.User.Get(testUserId);

            //assert
            editedUser.Email.ShouldBeEquivalentTo(testUserVm.Email);
            editedUser.FirstName.ShouldBeEquivalentTo(testUserVm.FirstName);
            editedUser.LastName.ShouldBeEquivalentTo(testUserVm.LastName);
            editedUser.BirthDate.ShouldBeEquivalentTo(testUserVm.BirthDate);
        }

        [Test]
        public void Cannot_Edit_Account_Role_And_LiveState() {
            //arrange
            var testUserId = _repoUnit.User.Get(user => user.Email == "00000@gmail.com").Id;
            var testUserVm = new UserEditVm(testUserId, "Toto", "Koko", "99999@gmail.com", Roles.Admin, new DateTime(1991, 09, 20), LiveState.Deleted);

            //act
            _accountService.EditUser(testUserVm);
            var editedUser = _repoUnit.User.Get(testUserId);

            //assert
            editedUser.Role.ShouldBeEquivalentTo(Roles.Simple);
            editedUser.LiveState.ShouldBeEquivalentTo(LiveState.Active);
        }
    }
}
