using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Core;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class BirthdayLoaderTest
    {
        private RepoUnit _unit;
        private UsersRules _rules;
        private SequentialGenerator<DateTime> _generator;

        [SetUp]
        public void SetUp()
        {
            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();

            _unit = new RepoUnit();
            _rules = new UsersRules(_unit);
            _generator = new SequentialGenerator<DateTime> { Direction = GeneratorDirection.Ascending };
        }

        [TestCase(1, 1, 31, Result = 31)]
        [TestCase(3, 5, 3, Result = 3)]
        [TestCase(15, 1, 31, Result = 31)]
        [TestCase(20, 5, 100, Result = 100)]
        [TestCase(1, 1, 300, Result = 300)]
        public int LoadUsersByBirthday_Should_Return_Users_Within_Range(int startDay, int startMonth, int dayCount) {
            // arrange
            _generator.StartingWith(new DateTime(1, startMonth, startDay));
            var endDate = new DateTime(1, 1, startDay).AddDays(dayCount);

            var users = Builder<User>.CreateListOfSize(dayCount).All()
                                     .With(x => x.LiveState = LiveState.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));
            
            // act 
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, startMonth, startDay), endDate);

            // assert
            return bornUsers.Count();
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_When_Passed_Dates_Are_Equal()
        {
            //arrange
            var users = Builder<User>.CreateListOfSize(2).All()
                                     .With(x => x.LiveState = LiveState.Active)
                                     .With(x => x.BirthDate = new DateTime(1, 08, 20))
                                     .Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 8, 20), new DateTime(1, 8, 20));

            //assert
            users.Count().Should().Be(2);
        }

        [Test]
        [TestCase(01, 09, 31, 08, Result = 12)]
        [TestCase(20, 09, 19, 09, Result = 12)]
        [TestCase(1, 06, 1, 01, Result = 8)]
        public int LoadUsersByBirthday_Should_Return_Users_When_Date_Period_Crosses_Over_Year(int startDay, int startMonth, int endDay, int endMonth)
        {
            // arrange
            for (int i = 1; i <= 12; i++)
            {
                _unit.User.Save(new User{LiveState = LiveState.Active, BirthDate = new DateTime(1, i, 1)});
            }

            // act 
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, startMonth, startDay), new DateTime(1, endMonth, endDay));

            // assert
            return bornUsers.Count();
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_One_User_With_Birthdate_On_The_29OfFebruary() {
            // arrange
            _unit.User.Save(new User{BirthDate = new DateTime(2016, 02, 29)});

            // act 
            var users = _rules.LoadUsersByBirthday(new DateTime(2990, 2, 1), new DateTime(3013, 3, 1));

            // assert
            users.Count().Should().Be(1);
        }

  
    }
}
