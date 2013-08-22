using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using Moq;
using NUnit.Framework;
using FluentAssertions;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class UserFilterTest
    {
        private UserService _userService;
        private FakeConfig _config;

        private List<User> _users;
        private List<User> _usersForStringSearchAndFiltering = new List<User>
            {
                new User {Email = "12345@gmail.com", FullName = "Saveli Bondini", FirstName = "Saveli", LastName = "Bondini", Role = Roles.Simple, ApproveState = ApproveStates.Approved, Live = LiveStatuses.Active},
                new User {Email = "5678@gmail.com", FullName = "Dima Rossi", FirstName = "Dima", LastName = "Rossi", Role = Roles.Simple, ApproveState = ApproveStates.Approved, Live = LiveStatuses.Active},
                new User {Email = "9999@gmail.com", FullName = "Dima Prohorov", FirstName = "Dima", LastName = "Prohorov", Role = Roles.Simple, ApproveState = ApproveStates.Approved, Live = LiveStatuses.Active}
            };

        private Dictionary<string, User> _usersForRoleAndStateFilteringTest = new Dictionary<string, User>
            {
                { "ACTIVE_APPROVED_ADMIN", new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved, Role = Roles.Admin }},
                { "ACTIVE_APPROVED_SIMPLE", new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved, Role = Roles.Simple }},
                { "ACTIVE_NOTAPPROVED_ADMIN", new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.NotApproved, Role = Roles.Admin }},
                { "ACTIVE_NOTAPPROVED_SIMPLE", new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.NotApproved, Role = Roles.Simple }},
                { "DELETED_APPROVED_ADMIN", new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.Approved, Role = Roles.Admin }},
                { "DELETED_APPROVED_SIMPLE", new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.Approved, Role = Roles.Simple }},
                { "DELETED_NOTAPPROVED_ADMIN", new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.NotApproved, Role = Roles.Admin }},
                { "DELETED_NOTAPPROVED_SIMPLE", new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.NotApproved, Role = Roles.Simple }}
            };

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            FakeDi.Register();

            _config = Config.Instance as FakeConfig;

            var repoUnit = new RepoUnit();
            Ioc.RegisterInstance<RepoUnit>(repoUnit);

            _userService = new UserService(repoUnit, Ioc.Resolve<ContactService>());
            Ioc.RegisterInstance<UserService>(_userService);
        }

        public void Setup(List<User> users)
        {
            _users = users;

            var repoUnit = Ioc.Resolve <RepoUnit>();
            repoUnit.User.Dispose();
            _users.ForEach(repoUnit.User.Save);

            _config.PageSize = _users.Count;
        }

        public void Setup<TSource>(Dictionary<TSource, User> users)
        {
            Setup(users.Values.ToList());
        }

        [Test,
        TestCase("12345@gmail.com"),
        TestCase("5678@gmail.com"),
        TestCase("9999@gmail.com")]
        public void Should_return_one_user_When_filter_contains_exiting_email_string(string email)
        {
            //arrange
            Setup(_usersForStringSearchAndFiltering);

            //act
            var users = _userService.RetreiveList(new UserFilterVm { SearchString = email }).Users;

            //assert
            users.Should().ContainSingle(user => user.Email == email);
        }

        [Test,
        TestCase("Ro"),
        TestCase("Dima"),
        TestCase("Dima Prohorov")]
        public void Should_return_users_having_specified_name_contained_in_full_name_not_contained_in_their_email_strings(string nameOrSurname)
        {
            //arrange
            Setup(_usersForStringSearchAndFiltering);

            //act
            var users = _userService.RetreiveList(new UserFilterVm { SearchString = nameOrSurname }).Users;

            //assert
            users.Should().OnlyContain(user => user.FullName.Contains(nameOrSurname));
        }

        [Test,
        TestCase("AbsentName"),
        TestCase("absent@absent.com")]
        public void Should_not_return_users_When_search_string_does_not_contained_both_in_email_and_full_name(string searchString)
        {
            //arrange
            Setup(_usersForStringSearchAndFiltering);

            //act
            var users = _userService.RetreiveList(new UserFilterVm { SearchString = searchString }).Users;

            //assert
            users.Should().BeEmpty();
        }

        [Test,
        TestCase(null),
        TestCase(""),
        TestCase(" "),
        TestCase("       ")]
        public void Should_return_all_users_When_filter_by_empty_string(string searchString)
        {
            //arrange
            Setup(_usersForStringSearchAndFiltering);

            //act
            var users = _userService.RetreiveList(new UserFilterVm { SearchString = searchString }).Users;

            //assert
            users.ShouldAllBeEquivalentTo(_users);
        }

        [Test,
        TestCase(true, true, true, new[] { "ACTIVE_APPROVED_ADMIN", "ACTIVE_NOTAPPROVED_ADMIN", "DELETED_APPROVED_ADMIN", "DELETED_NOTAPPROVED_ADMIN" }),
        TestCase(false, true, true, new[] { "ACTIVE_APPROVED_ADMIN", "ACTIVE_NOTAPPROVED_ADMIN" }),
        TestCase(true, true, false, new[]
        {
            "ACTIVE_APPROVED_ADMIN", "ACTIVE_APPROVED_SIMPLE",
            "ACTIVE_NOTAPPROVED_ADMIN", "ACTIVE_NOTAPPROVED_SIMPLE",
            "DELETED_APPROVED_ADMIN", "DELETED_APPROVED_SIMPLE",
            "DELETED_NOTAPPROVED_ADMIN", "DELETED_NOTAPPROVED_SIMPLE"
        }),
        TestCase(false, true, false, new[] { "ACTIVE_APPROVED_ADMIN", "ACTIVE_APPROVED_SIMPLE", "ACTIVE_NOTAPPROVED_ADMIN", "ACTIVE_NOTAPPROVED_SIMPLE" }),
        TestCase(true, false, true, new[] { "ACTIVE_APPROVED_ADMIN", "DELETED_APPROVED_ADMIN" }),
        TestCase(false, false, true, new[] { "ACTIVE_APPROVED_ADMIN" }),
        TestCase(true, false, false, new[] { "ACTIVE_APPROVED_ADMIN", "ACTIVE_APPROVED_SIMPLE", "DELETED_APPROVED_ADMIN", "DELETED_APPROVED_SIMPLE" }),
        TestCase(false, false, false, new[] { "ACTIVE_APPROVED_ADMIN", "ACTIVE_APPROVED_SIMPLE" })]
        public void Should_return_records_corresponding_to_selected_role_and_state_filters(bool showDeleted, bool showNotApproved, bool onlyAdmins, string[] expected)
        {
            //arrange
            Setup(_usersForRoleAndStateFilteringTest);

            var expectedResult = expected.Select(key => _usersForRoleAndStateFilteringTest[key]);

            //act
            var listPage = _userService.RetreiveList(new UserFilterVm
                                                         {
                                                             Deleted = showDeleted,
                                                             OnlyAdmins = onlyAdmins,
                                                             NotApproved = showNotApproved
                                                         }).Users;

            //assert
            listPage.ShouldAllBeEquivalentTo(expectedResult);
        }
    }
}
