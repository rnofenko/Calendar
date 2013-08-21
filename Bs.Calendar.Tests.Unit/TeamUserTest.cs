using System.Collections.Generic;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using System.Linq;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class TeamUserTest
    {
        private RepoUnit _unit;
        private TeamService _teamService;

        [SetUp]
        public void SetUp()
        {
            FakeDi.Register();

            _unit = new RepoUnit();
            Ioc.RegisterInstance<RepoUnit>(_unit);

            _teamService = new TeamService(_unit);
            Ioc.RegisterInstance<TeamService>(_teamService);
        }

        [Test]
        public void EditTeam_Should_Add_User_To_The_Team()
        {
            //arrange
            var user = new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 1};
            var team = new Team { Name = ".NET", Id = 1 };
            var teamVm = new TeamEditVm(team) { Users = new List<UserVm> { new UserVm(user) } };

            _unit.User.Save(user);
            _unit.Team.Save(team);         

            //act
            _teamService.EditTeam(teamVm);

            //assert
            _unit.Team.Get(1).Users.Count.Should().Be(1);
        }

        [Test]
        public void EditTeam_Should_Add_Many_Users_To_The_Team() 
        {
            //arrange
            var users = new List<User>
            {
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 1},
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 2}
            };
            var team = new Team {Name = ".NET", Id = 1};
            var teamVm = new TeamEditVm(team) { Users = users.Select(u => new UserVm(u)).ToList()};

            users.ForEach(u => _unit.User.Save(u));
            _unit.Team.Save(team);

            //act
            _teamService.EditTeam(teamVm);

            //assert
            _unit.Team.Get(1).Users.Count.Should().Be(2);
        }

        [Test]
        public void CreateTeam_Should_Add_User_To_The_Team() 
        {
            //arrange
            var user = new User { Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 1 };
            var teamVm = new TeamEditVm { Name = ".NET", Users = new List<UserVm> { new UserVm(user) } };
            _unit.User.Save(user);

            //act
            _teamService.CreateTeam(teamVm);

            //assert
            _unit.Team.Load(t => t.Name == ".NET").First().Users.Count.Should().Be(1);
        }

        [Test]
        public void CreateTeam_Should_Add_Many_Users_To_The_Team() 
        {
            //arrange
            var users = new List<User>
            {
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 1},
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 2}
            };
            var teamVm = new TeamEditVm { Name = ".NET", Users = users.Select(u => new UserVm(u)).ToList() };
            users.ForEach(u => _unit.User.Save(u));

            //act
            _teamService.CreateTeam(teamVm);

            //assert
            _unit.Team.Load(t => t.Name == ".NET").First().Users.Count.Should().Be(2);
        }

        [Test]
        public void EditTeam_Should_Edit_Team_When_No_Users_Are_Added()
        {
            //arrange
            _unit.Team.Save(new Team { Name = ".NET", Id = 1 });

            var teamVm = new TeamEditVm { TeamId = 1, Name = "C++", Users = null};

            //act
            _teamService.EditTeam(teamVm);

            //assert
            _unit.Team.Get(1).Name.ShouldBeEquivalentTo("C++");
        }

        [Test]
        public void CreateTeam_Should_Create_Team_When_No_Users_Are_Added() 
        {
            //arrange
            var teamVm = new TeamEditVm { Name = "C++", Users = null };

            //act
            _teamService.CreateTeam(teamVm);

            //assert
            _unit.Team.Get(t => t.Name == "C++").Should().NotBeNull();
        }


        [Test]
        public void EditTeam_Should_Delete_User_From_Team()
        {
            //arrange
            var users = new List<User>
            {
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 1},
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 2}
            };
            var team = new Team {Name = ".NET", Id = 1, Users = users};
            var teamVm = new TeamEditVm(team) { Users = new List<UserVm> { new UserVm(users[0]) } };

            users.ForEach(u => _unit.User.Save(u));
            _unit.Team.Save(team);

            //act
            _teamService.EditTeam(teamVm);

            //assert
            _unit.Team.Get(1).Users.Count.ShouldBeEquivalentTo(1);
        }

        [Test]
        public void EditTeam_Should_Delete_Many_Users_From_Team()
        {
            //arrange
            var users = new List<User>
            {
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 1},
                new User {Live = LiveStatuses.Active, FirstName = "Alex", LastName = "Al", Id = 2}
            };
            var team = new Team { Name = ".NET", Id = 1, Users = users };
            var teamVm = new TeamEditVm(team) { Users = new List<UserVm>()};

            users.ForEach(u => _unit.User.Save(u));
            _unit.Team.Save(team);

            //act
            _teamService.EditTeam(teamVm);

            //assert
            _unit.Team.Get(1).Users.Count.ShouldBeEquivalentTo(0);
        }
    }
}
