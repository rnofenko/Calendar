using System;
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
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture] 
    class TeamFilterTest
    {
        private TeamService _teamService;
        private List<Team> _teams;
        private RepoUnit _repoUnit;

        [TestFixtureSetUp]
        public void Setup()
        {
            DiMvc.Register();

            Ioc.RegisterType<IConfig, FakeConfig>();
            Ioc.RegisterType<ITeamRepository, FakeTeamRepository>();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();

            _repoUnit = new RepoUnit();
            Ioc.RegisterInstance<RepoUnit>(_repoUnit);

            _teams = new List<Team>
            {
                new Team {Name = ".NET", Description = ".NET team"},
                new Team {Name = ".PHP", Description = ".PHP team"},
                new Team {Name = ".C++", Description = ".C++ team"}
            };

            _teams.ForEach(_repoUnit.Team.Save);

            _teamService = new TeamService(_repoUnit);
            Ioc.RegisterInstance<TeamService>(_teamService);
        }


        [Test]
        public void Should_return_team_When_filter_by_name_of_existing_team()
        {
            //arrange
            var testedTeam = _teams[0];
            var filterVm = new TeamFilterVm { SearchString = testedTeam.Name };

            var expectedList = new[] { testedTeam };
            
            //act
            var teams = _teamService.RetreiveList(filterVm).Teams;

            //assert
            teams.ShouldAllBeEquivalentTo(expectedList);
        }

        [Test,
        TestCase(".")]
        public void Should_return_many_teams_When_filter_by_nonempty_similar_name(string name)
        {
            //arrange
            var filterVm = new TeamFilterVm { SearchString = name };

            //act
            var teams = _teamService.RetreiveList(filterVm).Teams;

            //assert
            teams.ShouldAllBeEquivalentTo(_teams);
        }

        [Test,
        TestCase("ThisTeamDoesn'tExists")]
        public void Should_return_no_team_When_filter_by_nonexistent_not_empty_name(string searchString)
        {
            //arrange
            var filterVm = new TeamFilterVm { SearchString = searchString };

            //act
            var teams = _teamService.RetreiveList(filterVm).Teams;

            //assert
            teams.Should().BeEmpty();
        }

        [Test,
        TestCase(null),
        TestCase(""),
        TestCase(" "),
        TestCase("   ")]
        public void Should_Return_All_Teams_When_Filter_By_Empty_String(string searchString)
        {
            //arrange
            var filterVm = new TeamFilterVm { SearchString = searchString };

            //act
            var teams = _teamService.RetreiveList(filterVm).Teams;

            //assert
            teams.ShouldAllBeEquivalentTo(_teams);
        }
    }
}
