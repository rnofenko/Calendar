using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
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
        private List<User> _users;
        private RepoUnit _repoUnit;

        [TestFixtureTearDown]
        public void TearDown()
        {
            Ioc.RegisterInstance<RepoUnit>(new RepoUnit());
        }

        private List<User> _usersForStringSearchAndFiltering = new List<User>
            {
                new User {Email = "12345@gmail.com", FullName = "Saveli Bondini", FirstName = "Saveli", LastName = "Bondini"},
                new User {Email = "5678@gmail.com", FullName = "Dima Rossi", FirstName = "Dima", LastName = "Rossi"},
                new User {Email = "9999@gmail.com", FullName = "Dima Prohorov", FirstName = "Dima", LastName = "Prohorov"}
            };

        private List<User> _usersForRoleAndStateFilteringTest = new List<User>
            {
                new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved, Role = Roles.Admin },
                new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.Approved, Role = Roles.Simple },
                new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.NotApproved, Role = Roles.Admin },
                new User{ Live = LiveStatuses.Active, ApproveState = ApproveStates.NotApproved, Role = Roles.Simple },
                new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.Approved, Role = Roles.Admin },
                new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.Approved, Role = Roles.Simple },
                new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.NotApproved, Role = Roles.Admin },
                new User{ Live = LiveStatuses.Deleted, ApproveState = ApproveStates.NotApproved, Role = Roles.Simple }
            };

        private void Setup(List<User> users)
        {
            _users = users.ToList(); //Create list copy for test independency

            //var moq = new Mock<IUserRepository>();
            
            //moq.Setup(m => m.Load()).Returns(_users.AsQueryable());

            //DiMvc.Register();
            //Ioc.RegisterInstance<IUserRepository>(moq.Object);
            //_userService = Ioc.Resolve<UserService>();

            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();

            _repoUnit = new RepoUnit();
            _users.ForEach(user => _repoUnit.User.Save(user));

            Ioc.RegisterInstance<RepoUnit>(_repoUnit);
            _userService = Ioc.Resolve<UserService>();
        }

        [Test]
        public void Should_Return_One_User_When_Filter_Has_Email()
        {
            //arrange

            Setup(_usersForStringSearchAndFiltering);

            var testEmail = _users[0].Email;
            var pagingVm = new PagingVm(true, true, true, true, true, true) { SearchStr = testEmail };

            //act
            var users = _userService.RetreiveList(pagingVm).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(1);
            users.First().Email.ShouldBeEquivalentTo(testEmail);
        }


        [Test]
        public void Should_Return_User_When_Filter_By_Name()
        {
            //arrange

            Setup(_usersForStringSearchAndFiltering);

            var testUser = _users[0];
            var pagingVm = new PagingVm(true, true, true, true, true, true) { SearchStr = testUser.FirstName };

            //act
            var users = _userService.RetreiveList(pagingVm).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(1);
            users.First().Email.ShouldBeEquivalentTo(testUser.Email);
            users.First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
        }

        [Test]
        public void Should_return_all_users_with_full_name_containing_specified_name_When_search_by_name() 
        {
            //arrange

            Setup(_usersForStringSearchAndFiltering);

            var testUser = _users[1];
            var pagingVm = new PagingVm(true, true, true, true, true, true) { SearchStr = testUser.FirstName };

            //act
            
            var users = _userService.RetreiveList(pagingVm).Users;

            //assert
            
            users.Count().ShouldBeEquivalentTo(2);

            users.First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
            users.Skip(1).First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
        }


        [Test]
        public void Should_Return_No_User_When_Filter_By_Nonexistent_Email() 
        {
            //arrange

            Setup(_usersForStringSearchAndFiltering);

            var pagingVm = new PagingVm { SearchStr = "00000@gmail.com" };

            //act
            var users = _userService.RetreiveList(pagingVm).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(0);
        }


        [Test]
        public void Should_Return_No_User_When_Filter_By_Nonexistent_Name() 
        {
            //arrange

            Setup(_usersForStringSearchAndFiltering);

            var pagingVm = new PagingVm { SearchStr = "Alex" };

            //act
            var users = _userService.RetreiveList(pagingVm).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(0);
        }


        [Test]
        public void Should_Return_All_Users_When_Filter_By_Empty_String()
        {
            //arrange

            Setup(_usersForStringSearchAndFiltering);

            var pagingVm = new PagingVm(true, true, true, true, true, true) { SearchStr = string.Empty};

            //act
            var users = _userService.RetreiveList(pagingVm).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(_users.Count);
        }

        [Test,
        TestCase(true, true, true, new []{ 0, 1, 2, 3, 4, 5, 6, 7 }),
        TestCase(false, true, true, new []{ 0, 1, 2, 3}),
        TestCase(true, true, false, new []{ 1, 3, 5, 7 }),
        TestCase(false, true, false, new []{ 1, 3 }),
        TestCase(true, false, true, new []{ 0, 1, 4, 5 }),
        TestCase(false, false, true, new []{ 0, 1 }),
        TestCase(true, false, false, new []{ 1, 5 }),
        TestCase(false, false, false, new []{1})
        ]
        //showDeleted, showNotApproved, showAdmins
        public void Should_return_records_corresponding_to_selected_role_and_state_filters(bool showDeleted, bool showNotApproved, bool showAdmins, int[] expected)
        {
            //arrange

            Setup(_usersForRoleAndStateFilteringTest);

            var pagingVm = new PagingVm(showDeleted, showAdmins, showNotApproved) { SearchStr = string.Empty };

            _userService.PageSize = _users.Count(); //Don't take paging filter in count

            var expectedResult = _users.Where((user, index) => expected.Contains(index)); //Select correct records in the right order

            //act

            var listPage = _userService.RetreiveList(pagingVm).Users;

            //assert

            listPage.ShouldAllBeEquivalentTo(expectedResult);
        }
    }
}
