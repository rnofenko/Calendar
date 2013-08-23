using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class LoggerTest
    {
        private RepoUnit _repository;
        private FakeConfig _config;

        private List<CalendarLog> _logEntities;

        [TestFixtureSetUp]
        private void SetUpFixture()
        {
            FakeDi.Register();

            _config = Config.Instance as FakeConfig;
        }

        [SetUp]
        private void Setup()
        {
            var repoUnit = new RepoUnit();
            Ioc.RegisterInstance<RepoUnit>(repoUnit);
            _logEntities.ForEach(repoUnit.CalendarLog.Save);
        }
    }
}
