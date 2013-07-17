using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class UsersControllerTest
    {
        private RepoUnit _unit;

        [TestFixtureSetUp]
        public void Setup(RepoUnit unit)
        {
            _unit = unit;
        }

        [Test]
        public void ShouldAddNewUserToTheDb()
        {
            // arrange 
            var newUser = new User
                {
                    Email = "newuser@gmail.com",
                    FirstName = "New",
                    LastName = "User",
                    Role = Roles.Simple
                };

            var userService = new UserService(_unit);
            var quantaty = userService.GetAllUsers().Count();
            
            // act
            userService.SaveUser(new UserEditVm(newUser));

            // assert
            Assert.AreEqual(userService.GetAllUsers().Count(), quantaty);
        }
    }
}
