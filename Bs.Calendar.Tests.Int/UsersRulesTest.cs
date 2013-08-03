using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    public class UsersRulesTest
    {
        private RepoUnit _unit;
        private IList<User> _users;
        private UsersRules _rules;

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Ioc.Resolve<RepoUnit>();
            _rules = new UsersRules(_unit);

            _users = Builder<User>.CreateListOfSize(100)
                .All()
                .With(u => u.LiveState = LiveState.Active)
                .With(u => u.BirthDate = new DateTime(1234, 1, 1))
                .Random(30)
                .With(u => u.BirthDate = new DateTime(1991, 3, 20))
                .Random(20)
                .With(u => u.BirthDate = new DateTime(1990, 4, 25))
                .Random(10)
                .With(u => u.BirthDate = new DateTime(1989, 3, 10))
                .Random(1)
                .With(u => u.BirthDate = new DateTime(2016, 2, 29))
                .Build();

            foreach (var user in _users)
            {
                _unit.User.Save(user);
            }
        }

        [Test]
        public void ShouldLoadUsersByBdays()
        {
            // arrange
            var from = new DateTime(2990, 3, 1);
            var into = new DateTime(3013, 5, 1);
            
            // act 
            var users = _rules.LoadUsersByBirthday(from, into);

            // assert
            users.Count().Should().Be(60);
        }

        [Test]
        public void ShouldLoadUsersByBdaysEvenIfIntoDateMonthLessThanFromDateMonth()
        {
            // arrange
            var from = new DateTime(2990, 5, 1);
            var into = new DateTime(3013, 3, 1);

            // act 
            var users = _rules.LoadUsersByBirthday(from, into);

            // assert
            users.Count().Should().Be(40);
        }

        [Test]
        public void ShouldReturnOneUserWithBirthdateOnThe29OfFebruary()
        {
            // arrange
            var from = new DateTime(2990, 2, 1);
            var into = new DateTime(3013, 3, 1);

            // act 
            var users = _rules.LoadUsersByBirthday(from, into);

            // assert
            users.FirstOrDefault().BirthDate.Should().Be(new DateTime(2016,2,29));
            users.Count().Should().Be(1);
        }
    }
}
