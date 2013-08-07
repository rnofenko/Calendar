using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
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

        private List<User> _usersForStringSearchAndFiltering = new List<User>
            {
                new User {Email = "12345@gmail.com", FullName = "Saveli Bondini", FirstName = "Saveli", LastName = "Bondini"},
                new User {Email = "5678@gmail.com", FullName = "Dima Rossi", FirstName = "Dima", LastName = "Rossi"},
                new User {Email = "9999@gmail.com", FullName = "Dima Prohorov", FirstName = "Dima", LastName = "Prohorov"}
            };

        private List<User> _usersForRoleAndStateFilteringTest = new List<User>
            {
                new User {LiveState = LiveState.Active, Role = Roles.Admin},
                new User {LiveState = LiveState.Active, Role = Roles.Simple},
                new User {LiveState = LiveState.Deleted, Role = Roles.Admin},
                new User {LiveState = LiveState.Deleted, Role = Roles.Simple},
                new User {LiveState = LiveState.NotApproved, Role = Roles.Admin},
                new User {LiveState = LiveState.NotApproved, Role = Roles.Simple}
            };

        private void Setup(List<User> users)
        {
            _users = users;

            var moq = new Mock<IUserRepository>();
            //Attention. In UserRepository::Load() we've got filtering by LiveState and here we take all records. Need to be corrected.
            moq.Setup(m => m.Load()).Returns(_users.AsQueryable());

            DiMvc.Register();
            Ioc.RegisterInstance<IUserRepository>(moq.Object);
            _userService = Ioc.Resolve<UserService>();
        }

        [Test]
        public void Should_Return_One_User_When_Filter_Has_Email()
        {
            //arrange

            Setup(_usersForRoleAndStateFilteringTest);

            var testEmail = _users[0].Email;
            var pagingVm = new PagingVm {SearchStr = testEmail};

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

            Setup(_usersForRoleAndStateFilteringTest);

            var testUser = _users[0];
            var pagingVm = new PagingVm { SearchStr = testUser.FirstName};

            //act
            var users = _userService.RetreiveList(pagingVm).Users;

            //assert
            users.Count().ShouldBeEquivalentTo(1);
            users.First().Email.ShouldBeEquivalentTo(testUser.Email);
            users.First().FirstName.ShouldBeEquivalentTo(testUser.FirstName);
        }

        [Test]
        public void Should_Return_Many_Users_When_Filter_By_Similar_Name() 
        {
            //arrange

            Setup(_usersForStringSearchAndFiltering);

            var testUser = _users[1];
            var pagingVm = new PagingVm { SearchStr = testUser.FirstName };

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

            var pagingVm = new PagingVm { SearchStr = string.Empty};

            //act
            var users = _userService.RetreiveList(pagingVm).Users;
            //assert
            users.Count().ShouldBeEquivalentTo(_users.Count);
        }

        [Test]
        public void Should_return_active_simple_users_When_IncludeNotApproved_is_false_and_IncludeAdmins_is_false()
        {
            //arrange

            Setup(_usersForRoleAndStateFilteringTest);

            var pagingVm = new PagingVm { SearchStr = string.Empty, IncludeNotApproved = false, IncludeAdmins = false };

            //act

            var listPage = _userService.RetreiveList(pagingVm).Users;

            //assert

            var rightResult = new User[] { _users[1] }; //Correct order of records in result

            listPage.ShouldAllBeEquivalentTo(rightResult);
        }

        [Test]
        public void Should_return_active_simple_and_admin_users_When_IncludeNotApproved_is_false_and_IncludeAdmins_is_true()
        {
            //arrange

            Setup(_usersForRoleAndStateFilteringTest);

            var pagingVm = new PagingVm { SearchStr = string.Empty, IncludeNotApproved = false, IncludeAdmins = true };

            //act

            var listPage = _userService.RetreiveList(pagingVm).Users;

            //assert

            var rightResult = new User[] { _users[0], _users[1] }; //Correct order of records in result

            listPage.ShouldAllBeEquivalentTo(rightResult);
        }

        [Test]
        public void Should_return_active_and_not_approved_simple_users_When_IncludeNotApproved_is_true_and_IncludeAdmins_is_false()
        {
            //arrange

            Setup(_usersForRoleAndStateFilteringTest);

            var pagingVm = new PagingVm { SearchStr = string.Empty, IncludeNotApproved = true, IncludeAdmins = false };

            //act

            var listPage = _userService.RetreiveList(pagingVm).Users;

            //assert

            var rightResult = new User[] { _users[1], _users[5] }; //Correct order of records in result

            listPage.ShouldAllBeEquivalentTo(rightResult);
        }

        [Test]
        public void Should_return_active_and_not_approved_simple_users_When_IncludeNotApproved_is_true_and_IncludeAdmins_is_true()
        {
            //arrange

            Setup(_usersForRoleAndStateFilteringTest);

            var pagingVm = new PagingVm { SearchStr = string.Empty, IncludeNotApproved = true, IncludeAdmins = true };

            //act

            var listPage = _userService.RetreiveList(pagingVm).Users;

            //assert

            var rightResult = new User[] { _users[0], _users[1], _users[4], _users[5] }; //Correct order of records in result

            listPage.ShouldAllBeEquivalentTo(rightResult);
        }

        [Test]
        public void Should_not_return_deleted_users(
            [Values(true, false)] bool IncludeNotApproved,
            [Values(true, false)] bool IncludeAdmins)
        {
            //arrange

            Setup(_usersForRoleAndStateFilteringTest);

            var pagingVm = new PagingVm { SearchStr = string.Empty, IncludeNotApproved = IncludeNotApproved, IncludeAdmins = IncludeAdmins };

            //act

            var listPage = _userService.RetreiveList(pagingVm).Users;

            //assert

            var excludedRecords = new User[] { _users[2], _users[3] }; //Records that must not be presented in result

            listPage.Should().NotIntersectWith(excludedRecords);
        }
    }
}
