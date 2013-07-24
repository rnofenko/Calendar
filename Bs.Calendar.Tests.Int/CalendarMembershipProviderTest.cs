﻿using System;
using System.Web.Security;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    public class CalendarMembershipProviderTest
    {
        private CalendarMembershipProvider _membershipProvider;        
        private RepoUnit _unit;
        private User _user;

        [TestFixtureSetUp]
        public void SetUp()
        {
            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Resolver.Resolve<RepoUnit>();
            _membershipProvider = new CalendarMembershipProvider();
            _user = new User
            {
                Email = "holmes@email.com",
                FirstName = "Sherlock", LastName = "Holmes",
                PasswordKeccakHash = "keccak", PasswordMd5Hash = "mdfive",
                Role = Roles.Simple
            };            
            _unit.User.Save(_user);           
        }

        [Test]
        public void ShouldReturnCorrectMembershipUserWithGivenEmail()
        {
            // act
            var user = _membershipProvider.GetUser(_user.Email, true);

            // assert
            user.Should().NotBeNull();
            user.UserName.Should().Be(string.Format("{0} {1}", _user.FirstName, _user.LastName));
            user.Email.Should().Be(_user.Email);           
        }

        [Test]
        public void ShouldReturnCorrectUserNameWithGivenEmail()
        {
            // assert
            string
                .Format("{0} {1}", _user.FirstName, _user.LastName)
                .Should()
                .Be(_membershipProvider.GetUserNameByEmail(_user.Email));
        }
    }
}
