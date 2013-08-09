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

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_Within_Month()
        {
            //arrange
            _generator.StartingWith(new DateTime(1, 1, 1));
            var users = Builder<User>.CreateListOfSize(31).All()
                                     .With(x => x.Live = LiveStatuses.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 1, 31));
            
            //assert
            bornUsers.Count().Should().Be(31);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_When_Period_Crosses_Monthes() {
            //arrange
            _generator.StartingWith(new DateTime(1, 1, 15));
            var users = Builder<User>.CreateListOfSize(31).All()
                                     .With(x => x.Live = LiveStatuses.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 15), new DateTime(1, 2, 28));

            //assert
            bornUsers.Count().Should().Be(31);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_Within_Many_Monthes() {
            //arrange
            _generator.StartingWith(new DateTime(15, 1, 1));
            var users = Builder<User>.CreateListOfSize(180).All()
                                     .With(x => x.Live = LiveStatuses.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 6, 30));

            //assert
            bornUsers.Count().Should().Be(180);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Not_Take_Care_About_Year() {
            //arrange
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(2033, 1, 02) });
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(3000, 1, 20) });

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 1, 30));

            //assert
            bornUsers.Count().Should().Be(2);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_Within_Year() {
            //arrange
            _generator.StartingWith(new DateTime(15, 1, 1));
            var users = Builder<User>.CreateListOfSize(365).All()
                                     .With(x => x.Live = LiveStatuses.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 12, 31));

            //assert
            bornUsers.Count().Should().Be(365);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_When_Period_Equals_One_Day()
        {
            //arrange
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(1, 01, 20) });
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(1, 01, 20) });

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 20), new DateTime(1, 1, 20));

            //assert
            bornUsers.Count().Should().Be(2);
        }


#warning no assert
        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_When_Date_Period_Crosses_Over_Year()
        {
            // arrange
            for (int i = 1; i <= 12; i++)
            {
                _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(1, i, 1) });
            }

            // act 
            var bornUsers1 = _rules.LoadUsersByBirthday(new DateTime(1, 06, 25), new DateTime(1, 06, 24));
            var bornUsers2 = _rules.LoadUsersByBirthday(new DateTime(1, 06, 1), new DateTime(1, 05, 31));

            // assert
            bornUsers1.Count().Should().Be(12);
            bornUsers2.Count().ShouldBeEquivalentTo(12);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_One_User_With_Birthdate_On_The_29OfFebruary() {
            // arrange
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(2016, 02, 29) });

            // act 
            var users = _rules.LoadUsersByBirthday(new DateTime(2990, 2, 1), new DateTime(3013, 3, 1));

            // assert
            users.Count().Should().Be(1);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_One_User_With_Birthdate_On_The_28OfFebruary() {
            // arrange
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(2015, 02, 28) });

            // act 
            var users = _rules.LoadUsersByBirthday(new DateTime(2990, 2, 1), new DateTime(3013, 3, 1));

            // assert
            users.Count().Should().Be(1);
        }

#warning year doesn't matter
        [Test]
        public void LoadUsersByBirthday_Should_Return_Empty_List_When_BirthDate_Is_Null()
        {
            //arrange
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = null });

            //act
            var users = _rules.LoadUsersByBirthday(DateTime.MinValue, DateTime.MaxValue);

            //assert
            users.Should().BeEmpty();
        }

        [Test]
        public void LoadUsersByBirthday_Should_Not_Return_Deleted_Users() {
            //arrange
            _generator.StartingWith(new DateTime(1, 1, 1));
            var users = Builder<User>.CreateListOfSize(31).All()
                                     .With(x => x.Live = LiveStatuses.Deleted)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));
            _unit.User.Save(new User { Live = LiveStatuses.Deleted, BirthDate = new DateTime(1, 1, 3)});

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 1, 31));

            //assert
            bornUsers.Count().ShouldBeEquivalentTo(1);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Consider_Last_Day_Of_Period() {
            //arrange
            _generator.StartingWith(new DateTime(1, 1, 20));
            var users = Builder<User>.CreateListOfSize(12).All()
                                     .With(x => x.Live = LiveStatuses.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 1, 31));

            //assert
            bornUsers.Count().Should().Be(12);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Consider_First_Day_Of_Period() {
            //arrange
            _generator.StartingWith(new DateTime(1, 1, 20));
            var users = Builder<User>.CreateListOfSize(2).All()
                                     .With(x => x.Live = LiveStatuses.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 20), new DateTime(1, 1, 21));

            //assert
            bornUsers.Count().Should().Be(2);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Not_Return_Users_Outside_Period() {
            //arrange
            _generator.StartingWith(new DateTime(1, 1, 1));
            var users = Builder<User>.CreateListOfSize(60).All()
                                     .With(x => x.Live = LiveStatuses.Active)
                                     .With(x => x.BirthDate = _generator.Generate()).Build().ToList();
            users.ForEach(user => _unit.User.Save(user));

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 1, 31));

            //assert
            bornUsers.Count().Should().Be(31);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Return_Users_Within_2_Day_Period_That_Crosses_Year() {
            //arrange
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(1, 12, 30) });
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(1, 12, 31) });
            _unit.User.Save(new User { Live = LiveStatuses.Active, BirthDate = new DateTime(1, 1, 1) });

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 12, 31), new DateTime(1, 1, 1));

            //assert
            bornUsers.Count().Should().Be(2);
        }

        [Test]
        public void LoadUsersByBirthday_Should_Not_Throw_Exception_And_Not_Load_Deleted_Users_When_Birthdate_Is_Null()
        {
            //arrange
            _unit.User.Save(new User { Live = LiveStatuses.Deleted, BirthDate = null });
            _unit.User.Save(new User { Live = LiveStatuses.Deleted, BirthDate = null });

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 1, 1), new DateTime(1, 12, 31));

            //assert
            bornUsers.Should().BeEmpty();
        }

        [Test]
        public void LoadUsersByBirthday_Should_Not_Return_User_Without_Birthdays()
        {
            //arrange
            _unit.User.Save(new User { Live = LiveStatuses.Deleted, BirthDate = new DateTime(1, 5, 09) });
            _unit.User.Save(new User { Live = LiveStatuses.Deleted, BirthDate = new DateTime(1, 5, 10) });
            _unit.User.Save(new User { Live = LiveStatuses.Deleted, BirthDate = new DateTime(1, 5, 20) });
            _unit.User.Save(new User { Live = LiveStatuses.Deleted, BirthDate = new DateTime(1, 5, 21) });

            //act
            var bornUsers = _rules.LoadUsersByBirthday(new DateTime(1, 5, 11), new DateTime(1, 5, 19));

            //assert
            bornUsers.Should().BeEmpty();
        }
    }
}
