using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Controllers;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Tests.Int.TestHelpers;
using Moq;
using System.Web;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class PagingPerfomanceTests
    {
        private UsersController _usersController;

        [TestFixtureTearDown]
        public void TearDown()
        {
            Database.SetInitializer(new FastDbInitializer(0));

            try
            {
                var _context = new CalendarContext();
                _context.Database.Initialize(true);
            }
            catch { /* Do nothing */ }
        }
        
        [TestFixtureSetUp]
        public void SetUp()
        {
            Database.SetInitializer(new FastDbInitializer(1000));

            try
            {
                var _context = new CalendarContext();
                _context.Database.Initialize(true);
            }
            catch { /* Do nothing */ }

            DiMvc.Register();
            Ioc.RegisterType<IUserRepository, UserRepository>();

            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);
            
            _usersController = Ioc.Resolve<UsersController>();
            _usersController.ControllerContext = mock.Object;
        }

        [Test]
        public void Estimate_Paging_Time()
        {
            //arrange

            var testedPagingModes = new PagingVm[]
                                         {
                                             new PagingVm(true, true, true, true, true, true) { Page = 100 },
                                             new PagingVm(true, true, true, true, true, true) { Page = 1000 },
                                             new PagingVm(true, true, true, true, true, true) { Page = 100000 }
                                         };

            var results = new double[testedPagingModes.Length];

            var stopWatch = new Stopwatch();

            //act

            //warm database up

            _usersController.List(new PagingVm(true, true, true, true, true, true) { Page = 1000 });

            for (int i = 0; i < testedPagingModes.Length; ++i)
            {
                stopWatch.Start();
                stopWatch.Stop();

                _usersController.List(testedPagingModes[i]);

                results[i] = stopWatch.Elapsed.TotalMilliseconds;
                stopWatch.Reset();
            }

            //assert

            for (int i = 0; i < results.Length; ++i)
            {
                Debug.WriteLine("Going to page {0} took {1} ms in time", testedPagingModes[i].Page, results[i]);
            }
        }

        [Test]
        public void Estimate_Sorting_And_Paging_Time()
        {
            //arrange

            var testedPagingModes = new PagingVm[]
                                         {
                                             new PagingVm(true, true, true, true, true, true) { SortByStr = "Name", Page = 100 },
                                             new PagingVm(true, true, true, true, true, true) { SortByStr = "Name", Page = 1000 },
                                             new PagingVm(true, true, true, true, true, true) { SortByStr = "Name", Page = 100000 }
                                         };

            var results = new double[testedPagingModes.Length];

            var stopWatch = new Stopwatch();

            //act

            //warm database up
            _usersController.List(new PagingVm(true, true, true, true, true, true) { SortByStr = "Name", Page = 1000 });

            for (int i = 0; i < testedPagingModes.Length; ++i)
            {
                stopWatch.Start();
                stopWatch.Stop();

                _usersController.List(testedPagingModes[i]);

                results[i] = stopWatch.Elapsed.TotalMilliseconds;
                stopWatch.Reset();
            }

            //assert

            for (int i = 0; i < results.Length; ++i)
            {
                Debug.WriteLine("Going to sorted page {0} took {1} ms in time", testedPagingModes[i].Page, results[i]);
            }
        }
    }
}

