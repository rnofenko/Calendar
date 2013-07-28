using System.Data.Entity;
using System.Diagnostics;
using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
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

        [TestFixtureSetUp]
        public void SetUp()
        {
            Database.SetInitializer(new FastDbInitializer(1000));
            try {
                var context = new CalendarContext();
                context.Database.Initialize(true);
            } catch {
                //Do Nothing
            }
             
            var mock = new Mock<ControllerContext>();
            mock.Setup(p => p.HttpContext.Session).Returns(new Mock<HttpSessionStateBase>().Object);

            DiMvc.Register();
            Resolver.RegisterType<IUserRepository, UserRepository>();

            _usersController = Resolver.Resolve<UsersController>();
            _usersController.ControllerContext = mock.Object;
        }

        [Test]
        public void Estimate_Paging_Time() 
        {
            //arrange
            var stopWatch = new Stopwatch();
            double page100, page10t, page500t;

            //act
            //warm database up
            _usersController.List(new PagingVm { Page = 1000});

            stopWatch.Start();
            _usersController.List(new PagingVm { Page = 100 });
            stopWatch.Stop();
            page100 = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch.Restart();
            _usersController.List(new PagingVm { Page = 10000 });
            stopWatch.Stop();
            page10t = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch.Restart();
            _usersController.List(new PagingVm { Page = 100000 });
            stopWatch.Stop();
            page500t = stopWatch.Elapsed.TotalMilliseconds;

            //assert
            Debug.WriteLine("Going to page 100 took {0} ms in time", page100);
            Debug.WriteLine("Going to page 10000 took {0} ms in time", page10t);
            Debug.WriteLine("Going to page 500000 took {0} ms in time", page500t);
        }

        [Test]
        public void Estimate_Sorting_And_Paging_Time()
        {
            //arrange
            var stopWatch = new Stopwatch();
            double page100, page10t, page500t;

            //act
            //warm database up
            _usersController.List(new PagingVm {SortByStr = "Name", Page = 1000});

            stopWatch.Start();
            _usersController.List(new PagingVm { SortByStr = "Name", Page = 100 });
            stopWatch.Stop();
            page100 = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch.Restart();
            _usersController.List(new PagingVm { SortByStr = "Name", Page = 10000 });
            stopWatch.Stop();
            page10t = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch.Restart();
            _usersController.List(new PagingVm { SortByStr = "Name", Page = 100000 });
            stopWatch.Stop();
            page500t = stopWatch.Elapsed.TotalMilliseconds;

            //assert
            Debug.WriteLine("Going to sorted page 100 took {0} ms in time", page100);
            Debug.WriteLine("Going to sorted page 10000 took {0} ms in time", page10t);
            Debug.WriteLine("Going to sorted page 500000 took {0} ms in time", page500t);
        }
    }
}
