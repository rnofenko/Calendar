using System.Linq;
using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture] 
    class TeamFrameTest
    {
        private RepoUnit _unit;
        private TeamController _teamController;

        [TestFixtureSetUp]
        public void SetUp() 
        {
            DiMvc.Register();
            Ioc.RegisterType<ITeamRepository, TeamRepository>();

            _unit = new RepoUnit();
            _unit.Team.Save(new Team { Name = ".NET", Description = ".NET"});
            _unit.Team.Save(new Team { Name = "PHP", Description = "PHP" });

            _teamController = new TeamController(new TeamService(_unit));
        }

        [TestFixtureTearDown]
        public void TearDown() 
        {
            var team1 = _unit.Team.Get(team => team.Name.Equals(".NET"));
            var team2 = _unit.Team.Get(team => team.Name.Equals("PHP"));

            _unit.Team.Delete(team1);
            _unit.Team.Delete(team2);
        }

        //[Test]
        //public void Can_Provide_All_Teams() 
        //{
        //    //act
        //    var teamView = _teamController.List(new PagingVm()) as PartialViewResult;
        //    var teams = teamView.Model as TeamsVm;

        //    //assert
        //    teams.Teams.Count().Should().BeGreaterOrEqualTo(2);
        //}

        //[Test]
        //public void Can_Search_Team() 
        //{
        //    //arrange
        //    var pagingVm = new PagingVm();
        //    pagingVm.SearchStr = ".NET";

        //    //act
        //    var teamsView = _teamController.List(pagingVm) as PartialViewResult;
        //    var teams = teamsView.Model as TeamsVm;
        //    var team = teams.Teams.First();

        //    //assert
        //    team.Name.ShouldBeEquivalentTo(pagingVm.SearchStr);
        //}
    }
}
