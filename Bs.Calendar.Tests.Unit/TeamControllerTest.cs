using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class TeamControllerTest
    {
        private TeamService _teamService;
        private TeamController _teamController;

        [TestFixtureSetUp]
        public void Setup() 
        {
            DiMvc.Register();
            Ioc.RegisterType<ITeamRepository, FakeTeamRepository>();

            var team = new Team
            {
                Name = ".NET",
                Description = ".NET TEAM"
            };

            _teamService = Ioc.Resolve<TeamService>();
            _teamService.CreateTeam(new TeamEditVm(team));
            _teamController = new TeamController(_teamService);
        }

        [Test]
        public void Should_Add_New_Team_To_Database() 
        {
            // arrange             
            var quantity = _teamService.GetAllTeams().Count();
            var team = new Team {
                Name = "NewOne",
                Description = "NONE"
            };

            // act
            _teamService.CreateTeam(new TeamEditVm(team));
            var count = _teamService.GetAllTeams().Count();

            // assert
            count.ShouldBeEquivalentTo(quantity + 1);
        }

        [Test]
        public void Should_Edit_Team() 
        {
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
        public void Should_Delete_Team() 
        {
            // arrange
            _teamService.CreateTeam(new TeamEditVm { Name = "LOGO", Description = "LOGO" });
            var teamToDelete = _teamService.GetAllTeams().First(team => team.Name.Equals("LOGO"));

            // act
            var viewResult = _teamController.Delete(new TeamEditVm(teamToDelete));
            var deletedTeam = _teamService.GetTeam(teamToDelete.Id);

            // assert
            deletedTeam.ShouldBeEquivalentTo(null);
        }
    }
}
