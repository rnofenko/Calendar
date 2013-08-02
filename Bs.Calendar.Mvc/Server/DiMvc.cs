using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Server
{
    public class DiMvc
    {
        public static void Register()
        {
            DiDataAccess.Register();

            Resolver.RegisterType<IConfig, MvcConfig>();
            Resolver.RegisterType<ICryptoProvider, KeccakCryptoProvider>();
            Resolver.RegisterType<IControllerFactory, UnityControllerFactory>();

            Resolver.RegisterType<ICryptoProvider, SimpleCryptoProvider>();
            Resolver.RegisterType<ISaltProvider, RandomSaltProvider>();
        }
    }
}