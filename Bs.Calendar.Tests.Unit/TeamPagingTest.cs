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
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class TeamPagingTest
    {
        private TeamService _teamService;
        private List<Team> _teams;

        [TestFixtureSetUp]
        public void Setup() {
            _teams = new List<Team>
            {
                new Team {Name = ".NET", Description = ".NET team"},
                new Team {Name = ".PHP", Description = ".PHP team"},
                new Team {Name = ".C++", Description = ".C++ team"},
                new Team {Name = ".D", Description = ".D team"}
            };

            var moq = new Mock<ITeamRepository>();
            moq.Setup(m => m.Load()).Returns(_teams.AsQueryable());

            DiMvc.Register();
            Resolver.RegisterInstance<ITeamRepository>(moq.Object);
            _teamService = Resolver.Resolve<TeamService>();
        }

        [Test]
        public void Can_Paginate_Teams() {
            //arrange
            _teamService.PageSize = 2;

            //act
            var teamPage1 = _teamService.RetreiveList(new PagingVm { Page = 1 }).Teams;
            var teamPage2 = _teamService.RetreiveList(new PagingVm { Page = 2 }).Teams;

            //assert
            teamPage1.Count().ShouldBeEquivalentTo(_teamService.PageSize);
            teamPage2.Count().ShouldBeEquivalentTo(_teamService.PageSize);

            teamPage1.First().ShouldBeEquivalentTo(_teams[0]);
            teamPage1.Last().ShouldBeEquivalentTo(_teams[1]);

            teamPage2.First().ShouldBeEquivalentTo(_teams[2]);
            teamPage2.Last().ShouldBeEquivalentTo(_teams[3]);
        }

        [Test]
        public void Can_Sort_Teams() {
            //arrange
            _teamService.PageSize = _teams.Count;

            //act
            var teams = _teamService.RetreiveList(new PagingVm { SortByStr = "Name", Page = 1 }).Teams;

            //assert
            teams.Count().ShouldBeEquivalentTo(_teamService.PageSize);
            teams.First().ShouldBeEquivalentTo(_teams[2]);
            teams.Last().ShouldBeEquivalentTo(_teams[1]);
        }

        [Test]
        public void Can_Sort_Teams_On_Multiple_Pages() {
            //arrange
            _teamService.PageSize = 2;

            //act
            var teamPage1 = _teamService.RetreiveList(new PagingVm { SortByStr = "Name", Page = 1 }).Teams;
            var teamPage2 = _teamService.RetreiveList(new PagingVm { SortByStr = "Name", Page = 2 }).Teams;

            //assert
            teamPage1.First().ShouldBeEquivalentTo(_teams[2]);
            teamPage2.Last().ShouldBeEquivalentTo(_teams[1]);
        }

        [Test]
        public void Can_Paginate_Sort_And_Search_Teams() {
            //arrange
            _teamService.PageSize = 2;

            //act
            var usersPage2 = _teamService.RetreiveList(new PagingVm {
                SearchStr = ".NET",
                SortByStr = "Name",
                Page = 2
            }).Teams;

            //assert
            usersPage2.Count().ShouldBeEquivalentTo(1);
            usersPage2.First().Name.ShouldBeEquivalentTo(".NET");
        }
    }
}
