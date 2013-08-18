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
    class TeamPagingTest
    {
        private TeamService _teamService;
        private List<Team> _teams;
        private RepoUnit _repoUnit;

        private FakeConfig _config;

        [TestFixtureSetUp]
        public void Setup()
        {
            FakeDi.Register();

            _config = Config.Instance as FakeConfig;

            _teams = new List<Team>
            {
                new Team {Name = ".NET", Description = ".NET team"},
                new Team {Name = ".PHP", Description = ".PHP team"},
                new Team {Name = ".C++", Description = ".C++ team"},
                new Team {Name = ".D", Description = ".D team"}
            };

            _repoUnit = new RepoUnit();
            _teams.ForEach(_repoUnit.Team.Save);

            _teamService = new TeamService(_repoUnit);
            Ioc.RegisterInstance<TeamService>(_teamService);
        }

        [Test]
        public void Can_Paginate_Teams()
        {
            //arrange
            _config.PageSize = 2;

            var expectedPage1Content = new[] {_teams[0], _teams[1]};
            var expectedPage2Content = new[] {_teams[2], _teams[3]};

            //act
            var teamPage1 = _teamService.RetreiveList(new TeamFilterVm { Page = 1 }).Teams;
            var teamPage2 = _teamService.RetreiveList(new TeamFilterVm { Page = 2 }).Teams;

            //assert
            teamPage1.ShouldAllBeEquivalentTo(expectedPage1Content);
            teamPage2.ShouldAllBeEquivalentTo(expectedPage2Content);
        }

        [Test,
        TestCase("Name", new[] { ".C++", ".D", ".NET", ".PHP" }),
        TestCase("Name desc", new[] { ".PHP", ".NET", ".D", ".C++" })]
        public void Should_sort_teams_by_name_field(string sortByField, string[] expectedNames)
        {
            //arrange
            _config.PageSize = _teams.Count;

            //act
            var teams = _teamService.RetreiveList(new TeamFilterVm { SortByField = sortByField }).Teams;

            //assert
            teams.Select(team => team.Name).ShouldAllBeEquivalentTo(expectedNames);
        }

        [Test,
        TestCase("Name", new[] { ".C++", ".D" }, new[] { ".NET", ".PHP" }),
        TestCase("Name desc", new[] { ".PHP", ".NET" }, new[] { ".D", ".C++" })]
        public void Can_Sort_Teams_On_Multiple_Pages(string sortByField, string[] page1ExpectedNames, string[] page2ExpectedNames)
        {
            //arrange
            _config.PageSize = 2;

            //act
            var teamPage1 = _teamService.RetreiveList(new TeamFilterVm { SortByField = sortByField, Page = 1 }).Teams;
            var teamPage2 = _teamService.RetreiveList(new TeamFilterVm { SortByField = sortByField, Page = 2 }).Teams;

            //assert
            teamPage1.Select(team => team.Name).ShouldAllBeEquivalentTo(page1ExpectedNames);
            teamPage2.Select(team => team.Name).ShouldAllBeEquivalentTo(page2ExpectedNames);
        }

        [Test,
        TestCase(".NET", "Name", 2, 1, new[] { ".NET" }),
        TestCase(".NET", "Name", 2, 2, new string[] {}),
        TestCase(".NET", "Name desc", 2, 1, new[] { ".NET" }),
        TestCase(".NET", "Name desc", 2, 2, new string[] { }),
        TestCase(".", "Name", 1, 1, new[] { ".C++" }),
        TestCase(".", "Name", 2, 1, new[] { ".C++", ".D" }),
        TestCase(".", "Name", 3, 1, new[] { ".C++", ".D", ".NET" }),
        TestCase(".", "Name", 4, 1, new[] { ".C++", ".D", ".NET", ".PHP" }),
        TestCase(".", "Name", 1, 2, new[] { ".D" }),
        TestCase(".", "Name", 2, 2, new[] { ".NET", ".PHP" }),
        TestCase(".", "Name", 3, 2, new[] { ".PHP" }),
        TestCase(".", "Name", 4, 2, new string[] {})]
        public void Can_Paginate_Sort_And_Search_Teams(string searchString, string sortByField, int pageSize, int lookupPage, string[] expectedNames)
        {
            //arrange
            _config.PageSize = pageSize;
            var filterVm = new TeamFilterVm
                               {
                                   SearchString = searchString,
                                   SortByField = sortByField,
                                   Page = lookupPage
                               };

            //act
            var teams = _teamService.RetreiveList(filterVm).Teams;

            //assert
            teams.Select(team => team.Name).ShouldAllBeEquivalentTo(expectedNames);
        }
    }
}
