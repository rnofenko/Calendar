using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;

namespace Bs.Calendar.Tests.Unit.FakeObjects
{
    class FakeDi
    {
        public static void Register()
        {
            registerConfig();
            registerControllers();
            registerDataAccess();
            registerEmail();
            registerCryptography();
        }

        private static void registerDataAccess()
        {
            Ioc.RegisterType<IUserRepository, FakeUserRepository>();
            Ioc.RegisterType<IRoomRepository, RoomRepository>();
            Ioc.RegisterType<ITeamRepository, FakeTeamRepository>();
            Ioc.RegisterType<IBookRepository, BookRepository>();
            Ioc.RegisterType<IBookHistoryRepository, BookHistoryRepository>();
            Ioc.RegisterType<IPersonalEventRepository, PersonalEventRepository>();
            Ioc.RegisterType<ITeamEventRepository, TeamEventRepository>();
            Ioc.RegisterType<ICalendarLogRepository, FakeCalendarLogRepository>();

            Ioc.RegisterType<IEmailOnEventHistoryRepository, FakeEmailOnEventHistoryRepository>();
            Ioc.RegisterType<ITeamEventRepository, FakeTeamEventRepository>();
            Ioc.RegisterType<IPersonalEventRepository, FakePersonalEventRepository>();
        }

        private static void registerCryptography()
        {
            Ioc.RegisterType<ICryptoProvider, SimpleCryptoProvider>();
            Ioc.RegisterType<ISaltProvider, RandomSaltProvider>();
        }

        private static void registerControllers()
        {
            Ioc.RegisterType<IControllerFactory, UnityControllerFactory>();
        }

        private static void registerConfig()
        {
            Ioc.RegisterType<IConfig, FakeConfig>();
        }

        private static void registerEmail()
        {
            Ioc.RegisterType<IEmailProvider, StandardEmailProvider>();
        }
    }
}
