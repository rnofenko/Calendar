using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Core;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class BirthdayLoaderTest
    {
        private RepoUnit _unit;
        private IList<User> _users;
        private UsersRules _rules;

        [SetUp]
        public void SetUp()
        {
            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Ioc.Resolve<RepoUnit>();
            _rules = new UsersRules(_unit);

        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_When_Passed_Dates_Are_Equal()
        {
            //arrange
            var date = new DateTime(2013, 01, 31);
            var user = new User {BirthDate = new DateTime(1991, 01, 31)};
            _unit.User.Save(user);

            //act
            var users = _rules.LoadUsersByBirthday(date, date);

            //assert
            users.Should().Should().NotBeNull();
            users.Should().NotBeEmpty();
        }

        [Test]
        public void LoadUsersByBirthday_Should_Not_Return_Not_Yet_Borned_Users()
        {
            //arrange
            var user = new User { BirthDate = new DateTime(2022, 01, 27) };
            _unit.User.Save(user);

            //act
            var users = _rules.LoadUsersByBirthday(new DateTime(2013, 01, 01), new DateTime(2014, 01, 31));

            //assert
            users.Should().BeEmpty();
        }

        [Test]
        public void LoadUsersByBirthday_Should_Track_Year() {
            //arrange
            var user = new User { BirthDate = new DateTime(1991, 02, 27)};
            _unit.User.Save(user);

            //act
            var users = _rules.LoadUsersByBirthday(new DateTime(2013, 01, 01), new DateTime(2014, 01, 31));

            //assert
            users.Should().NotBeEmpty();
            users.First().BirthDate.ShouldBeEquivalentTo(user.BirthDate); 
        }
    }
}
