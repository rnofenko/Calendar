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
    class TeamFilterTest
    {
        private TeamService _teamService;
        private List<Team> _teams;

        [TestFixtureSetUp]
        public void Setup()
        {
            _teams = new List<Team>
            {
                new Team {Name = ".NET", Description = ".NET team"},
                new Team {Name = ".PHP", Description = ".PHP team"},
                new Team {Name = ".C++", Description = ".C++ team"}
            };

            var moq = new Mock<ITeamRepository>();
            moq.Setup(m => m.Load()).Returns(_teams.AsQueryable());

            DiMvc.Register();
            Ioc.RegisterInstance<ITeamRepository>(moq.Object);
            _teamService = Ioc.Resolve<TeamService>();
        }


        [Test]
        public void Should_Return_Team_When_Filter_By_Name() 
        {
            //arrange
            var pagingVm = new PagingVm { SearchStr = _teams[0].Name };

            //act
            var teams = _teamService.RetreiveList(pagingVm).Teams;

            //assert
            teams.Count().ShouldBeEquivalentTo(1);
            teams.First().Name.ShouldBeEquivalentTo(pagingVm.SearchStr);
        }

        [Test]
        public void Should_Return_Many_Teams_When_Filter_By_Similar_Name() 
        {
            //arrange
            var pagingVm = new PagingVm { SearchStr = "." };

            //act
            var teams = _teamService.RetreiveList(pagingVm).Teams;

            //assert
            teams.Count().ShouldBeEquivalentTo(_teams.Count);
        }


        [Test]
        public void Should_Return_No_Team_When_Filter_By_Nonexistent_Name() 
        {
            //arrange
            var pagingVm = new PagingVm { SearchStr = "PROLOG" };

            //act
            var teams = _teamService.RetreiveList(pagingVm).Teams;

            //assert
            teams.Count().ShouldBeEquivalentTo(0);
        }


        [Test]
        public void Should_Return_All_Teams_When_Filter_By_Empty_String() 
        {
            //arrange
            var pagingVm = new PagingVm { SearchStr = string.Empty };

            //act
            var teams = _teamService.RetreiveList(pagingVm).Teams;
            //assert
            teams.Count().ShouldBeEquivalentTo(_teams.Count);
        }
    }
}
