using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
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
