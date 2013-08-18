using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;

namespace Bs.Calendar.Mvc.Server
{
    public class DiMvc
    {
        public static void Register()
        {
            DiDataAccess.Register();

            Ioc.RegisterType<IConfig, MvcConfig>();
            Ioc.RegisterType<IControllerFactory, UnityControllerFactory>();

            Ioc.RegisterType<ICryptoProvider, KeccakCryptoProvider>();
            Ioc.RegisterType<ISaltProvider, RandomSaltProvider>();
            
            Ioc.RegisterType<IEmailProvider, StandardEmailProvider>();
        }
    }
}