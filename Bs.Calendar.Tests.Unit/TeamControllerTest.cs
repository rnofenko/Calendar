using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc;
using Bs.Calendar.Mvc.Controllers;
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
    class TeamControllerTest
    {
        private TeamService _teamService;
        private TeamController _teamController;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            DiMvc.Register();

            Ioc.RegisterType<ITeamRepository, FakeTeamRepository>();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();
            Ioc.RegisterType<IConfig, FakeConfig>();

            var team = new Team
            {
                Name = ".NET",
                Description = ".NET TEAM"
            };

            var repoUnit = new RepoUnit();
            Ioc.RegisterInstance<RepoUnit>(repoUnit);

            _teamService = new TeamService(repoUnit);
            Ioc.RegisterInstance<TeamService>(_teamService);

            _teamService.CreateTeam(new TeamEditVm(team));

            //Setup Team controller and mock http context
            var dummyRequestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            _teamController = Ioc.Resolve<TeamController>();
            _teamController.Url = new UrlHelper(dummyRequestContext);
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
