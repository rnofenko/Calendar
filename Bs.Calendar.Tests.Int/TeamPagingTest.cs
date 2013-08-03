using System.Linq;
using System.Web.Mvc;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class TeamPagingTest
    {
        private RepoUnit _unit;
        private TeamController _teamController;
        private int _pageSize;

        [TestFixtureSetUp]
        public void SetUp() {
            DiMvc.Register();
            Ioc.RegisterType<ITeamRepository, TeamRepository>();

            _unit = new RepoUnit();
            _unit.Team.Save(new Team { Name = ".NET", Description = ".NET" });
            _unit.Team.Save(new Team { Name = "PHP", Description = "PHP" });

            var teamService = new TeamService(_unit);
            teamService.PageSize = _pageSize = 1;

            _teamController = new TeamController(teamService);
        }

        [TestFixtureTearDown]
        public void TearDown() {
            var team1 = _unit.Team.Get(team => team.Name.Equals(".NET"));
            var team2 = _unit.Team.Get(team => team.Name.Equals("PHP"));

            _unit.Team.Delete(team1);
            _unit.Team.Delete(team2);
        }

        [Test]
        public void Can_Paginate_Teams() {
            //act
            var teamsView = _teamController.List(new PagingVm { Page = 2 }) as PartialViewResult;
            var teams = teamsView.Model as TeamsVm;

            //assert
            teams.Teams.Count().ShouldBeEquivalentTo(_pageSize);
        }

        [Test]
        public void Can_Sort_Teams() {
            //arrange
            var team = _unit.Team.Load().OrderBy(n => n.Name).First();

            //act
            var teamsView = _teamController.List(new PagingVm { Page = 1, SortByStr = "Name" }) as PartialViewResult;
            var teams = teamsView.Model as TeamsVm;

            //assert
            teams.Teams.Count().ShouldBeEquivalentTo(_pageSize);
            teams.Teams.First().ShouldBeEquivalentTo(team);
        }
    }
}
