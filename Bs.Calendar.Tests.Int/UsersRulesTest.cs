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
            Resolver.RegisterType<IUserRepository, FakeUserRepository>();
            _unit = Resolver.Resolve<RepoUnit>();
            _rules = new UsersRules(_unit);

            _users = Builder<User>.CreateListOfSize(100)
                .All()
                .With(u => u.LiveState = LiveState.Active)                
                .All()
                .With(u => u.BirthDate = new DateTime(1991, 1, 15))
                .Random(35)
                .With(u => u.BirthDate = new DateTime(1990, 7, 15))                
                .Random(15)
                .With(u => u.BirthDate = new DateTime(1989, 7, 10))
                .Random(10)
                .With(u => u.BirthDate = new DateTime(1992, 7, 20))
                .With(u => u.LiveState = LiveState.Deleted)
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
            var from = new DateTime(1990, 1, 1);
            var into = new DateTime(2013, 7, 20);                       
            
            // act 
            var users = _rules.LoadUsersByBirthday(from, into);

            // assert
            users.Count().Should().Be(75);
        }

        [Test]
        public void ShouldLoadUsersByBdaysFromGivenDateToTheEndOfMonth()
        {
            // arrange
            var from = new DateTime(2013, 7, 12);

            // act 
            var users = _rules.LoadUsersByBirthday(from);

            // assert
            users.Count().Should().Be(35);
        }
    }
}
