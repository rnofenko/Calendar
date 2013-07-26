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
    class TeamControllerTest
    {
        private RepoUnit _unit;
        private TeamService _teamService;
        private TeamController _teamController;

        [TestFixtureSetUp]
        public void SetUp() {
            DiMvc.Register();
            Resolver.RegisterType<ITeamRepository, TeamRepository>();

            _unit = new RepoUnit();
            _unit.Team.Save(new Team { Name = ".NET", Description = ".NET" });
            _unit.Team.Save(new Team { Name = "PHP", Description = "PHP" });

            _teamService = new TeamService(_unit);
            _teamController = new TeamController(_teamService);
        }

        [TestFixtureTearDown]
        public void TearDown() {
            var team1 = _unit.Team.Get(team => team.Name.Equals(".NET"));
            var team2 = _unit.Team.Get(team => team.Name.Equals("PHP"));

            _unit.Team.Delete(team1);
            _unit.Team.Delete(team2);
        }

        [Test]
        public void Should_Add_New_Team_To_Database() {
            // arrange             
            var quantity = _teamService.GetAllTeams().Count();
            var team = new Team {
                Name = "NewOne",
                Description = "NONE"
            };

            // act
            _teamService.SaveTeam(new TeamEditVm(team));
            var count = _teamService.GetAllTeams().Count();

            // assert
            count.ShouldBeEquivalentTo(quantity + 1);
        }

        [Test]
        public void Should_Show_Team_Details() {
            // arrange
            var team = _teamService.GetAllTeams().First();

            // act
            var viewResult = _teamController.Details(team.Id) as ViewResult;
            var model = viewResult.Model as TeamEditVm;

            // assert
            model.ShouldBeEquivalentTo(new TeamEditVm(team));
        }

        [Test]
        public void Should_Edit_Team() {
            // arrange
            var team = _teamService.GetAllTeams().Last();
            team.Name = "Modyfied";

            // act
            var viewResult = _teamController.Edit(new TeamEditVm(team));
            var teamResult = _teamService.GetTeam(team.Id);

            // assert
            teamResult.Id.ShouldBeEquivalentTo(team.Id);
        }

        [Test]
        public void Should_Delete_Team() {
            // arrange
            _teamService.SaveTeam(new TeamEditVm { Name = "LOGO", Description = "LOGO" });
            var teamToDelete = _teamService.GetAllTeams().First(team => team.Name.Equals("LOGO"));

            // act
            var viewResult = _teamController.Delete(new TeamEditVm(teamToDelete));
            var deletedTeam = _teamService.GetTeam(teamToDelete.Id);

            // assert
            deletedTeam.ShouldBeEquivalentTo(null);
        }
    }
}
