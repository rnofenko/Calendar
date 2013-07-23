using System.Web.Mvc;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;

namespace Bs.Calendar.Mvc.Server
{
    public class DiMvc
    {
        public static void Register()
        {
            DiDataAccess.Register();

            Resolver.RegisterType<IControllerFactory, UnityControllerFactory>();
        }
    }
}